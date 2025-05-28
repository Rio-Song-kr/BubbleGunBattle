Shader "BubbleR/StandardBubbleURP"
{
    Properties
    {
        [Header(Base)]
        _MainColor ("Color", Color) = (1,1,1,0.2)
        _Glossiness ("Smoothness", Range(0,1)) = 1.0
        _Metal ("Metallic", Range(0,1)) = 1.0

        [Header(Bouncing)]
        _BounceAmplitude ("Bounce Amplitude", Range(0,1)) = 0.2
        _BounceFrequency ("Bounce Frequency Multiplier", Range(0,5)) = 1

        [Header(Color Shifting)]
        _ColorShiftPeak ("Color Shifting Offset (RGB)", Vector) = (0.3, 0.5, 0.8)
        _ColorShiftBand ("Color Shifting Bands (RGB)", Vector) = (0.3, 0.2, 0.3)
        _ColorShifting ("Color Shift Multiplier", Range(0,1)) = 1.0
        _ColorShiftNoise ("Color Shift Noise (Multiplier, Scale, Speed, Unused)", Vector) = (0.1, 1, 1, 0)
        [Enum(Add, 0, Multiply, 1, Subtract, 2)] _ColorShiftMode ("Shift Mode", Float) = 0
        [Toggle(SPATIAL_NOISE)] _SpatialNoise ("Enable Spatial Noise", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        // Main Pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ SPATIAL_NOISE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // 쉐이더 프로퍼티
            float _BounceAmplitude, _BounceFrequency;
            float4 _MainColor;
            float _Glossiness, _Metal;
            float3 _ColorShiftPeak, _ColorShiftBand;
            float _ColorShifting;
            float4 _ColorShiftNoise;
            float _ColorShiftMode;

            // 인스턴싱 지원
            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            // 노이즈 함수
            inline float mod289(float x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            inline float4 mod289(float4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            inline float4 perm(float4 x) { return mod289(((x * 34.0) + 1.0) * x); }

            float GetNoise(float3 p) {
                float3 a = floor(p);
                float3 d = p - a;
                d = d * d * (3.0 - 2.0 * d);

                float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
                float4 k1 = perm(b.xyxy);
                float4 k2 = perm(k1.xyxy + b.zzww);

                float4 c = k2 + a.zzzz;
                float4 k3 = perm(c);
                float4 k4 = perm(c + 1.0);

                float4 o1 = frac(k3 * (1.0 / 41.0));
                float4 o2 = frac(k4 * (1.0 / 41.0));

                float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
                float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);

                return o4.y * d.y + o4.x * (1.0 - d.y);
            }

            struct appdata_bubble {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 color : TEXCOORD2;
                float2 uv : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float TimeFunction(float timeScale, float offset) {
                float t = offset + _Time.y * timeScale;
                float s = sin(t * 6) + sin(t * 2.72) + sin(t * 1.541);
                return s / 3.0;
            }

            float TransformVertex(inout appdata_bubble v) {
                UNITY_SETUP_INSTANCE_ID(v);
                float offset = 0;

            #ifdef UNITY_INSTANCING_ENABLED
                offset = unity_InstanceID * 0.631;
            #endif

                float vMul = TimeFunction(_BounceFrequency, offset) * _BounceAmplitude * (1 - v.texcoord.z);
                float hMul = -vMul;

                v.vertex.y += v.normal.y * vMul * 0.1;
                v.vertex.xz += v.normal.xz * hMul * 0.1;

                return vMul;
            }

            inline float Center(float val, float center, float range) {
                return 1 - saturate(pow(abs(val - center), 2) / (range * 0.5));
            }

            inline float NoiseFromNormal(float3 norm, float scale, float speed) {
                float t = _Time.y * speed;
                float val = sin(t + (0.17 + norm.x * 3.17 * scale));
                val += sin(t + (0.11 + norm.y * 2.57 * scale));
                val += sin(t + norm.z * 3.7 * scale);
                val += sin(t + norm.x * 0.45 + norm.y * 0.67 + norm.z * 1.46);
                return val;
            }

            float3 CalculateShiftColor(v2f i) {
                float3 basePos = 0;

            #ifdef SPATIAL_NOISE
                basePos = i.worldPos * _ColorShiftNoise.y;
            #endif

                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                float3 noiseVector = sin(0.15 + basePos * float3(0.231, 0.15, 0.16)) +
                                    (sin(basePos * float3(0.31, 0.5, 0.9) + normal * 2.1) + sin(basePos * 0.04 + normal));

                float angle = saturate(1 - abs(dot(viewDir, normal)));
                float colorShift = angle;

                float shiftNoise = GetNoise(basePos);
                float angularShiftNoise = NoiseFromNormal(noiseVector, _ColorShiftNoise.y, _ColorShiftNoise.z);

                colorShift += (angularShiftNoise + shiftNoise) * _ColorShiftNoise.x;

                _ColorShiftPeak = saturate(_ColorShiftPeak);
                _ColorShiftBand = max(0, _ColorShiftBand);

                float3 sCol = float3(
                    Center(colorShift, _ColorShiftPeak.x * _ColorShifting, _ColorShiftBand.x),
                    Center(colorShift, _ColorShiftPeak.y * _ColorShifting, _ColorShiftBand.y),
                    Center(colorShift, _ColorShiftPeak.z * _ColorShifting, _ColorShiftBand.z)
                );

                return sCol;
            }

            v2f vert(appdata_bubble v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                TransformVertex(v);
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.color = v.color;
                o.uv = v.texcoord.xy;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 albedo = _MainColor.rgb * i.color.rgb;
                float3 sCol = CalculateShiftColor(i);

                // 색상 변화 모드 적용
                if (_ColorShiftMode == 0) // ADD
                {
                    sCol = lerp(0, sCol, _ColorShifting);
                    albedo = albedo + sCol;
                }
                else if (_ColorShiftMode == 1) // MUL
                {
                    sCol = lerp(1, sCol, _ColorShifting);
                    albedo = albedo * sCol;
                }
                else if (_ColorShiftMode == 2) // SUB
                {
                    sCol = lerp(0, sCol, _ColorShifting);
                    albedo = albedo - sCol;
                }

                // URP 조명 계산 (간단한 Lambertian 모델)
                float3 normal = normalize(i.worldNormal);
                Light mainLight = GetMainLight();
                float3 lightDir = mainLight.direction;
                float NdotL = saturate(dot(normal, lightDir));
                float3 finalColor = albedo * NdotL * mainLight.color;

                // 알파 값 설정
                half alpha = _MainColor.a * i.color.a;

                return half4(finalColor, alpha);
            }
            ENDHLSL
        }

        // Shadow Caster Pass (Shadows.hlsl 없이)
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ SPATIAL_NOISE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _BounceAmplitude, _BounceFrequency;
            float4 _MainColor;

            struct appdata_bubble {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float TimeFunction(float timeScale, float offset) {
                float t = offset + _Time.y * timeScale;
                float s = sin(t * 6) + sin(t * 2.72) + sin(t * 1.541);
                return s / 3.0;
            }

            float TransformVertex(inout appdata_bubble v) {
                UNITY_SETUP_INSTANCE_ID(v);
                float offset = 0;

            #ifdef UNITY_INSTANCING_ENABLED
                offset = unity_InstanceID * 0.631;
            #endif

                float vMul = TimeFunction(_BounceFrequency, offset) * _BounceAmplitude * (1 - v.texcoord.z);
                float hMul = -vMul;

                v.vertex.y += v.normal.y * vMul * 0.1;
                v.vertex.xz += v.normal.xz * hMul * 0.1;

                return vMul;
            }

            v2f vert(appdata_bubble v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                TransformVertex(v);
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.texcoord.xy;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // 알파 클리핑으로 그림자 투명도 처리
                clip(_MainColor.a - 0.1);
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}