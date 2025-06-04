using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "Scriptable Objects/AudioDataSo")]
public class AudioDataSO : ScriptableObject
{
    [Header("Audio Mixer Groups")]
    public AudioMixerGroup BGMAudioMixer;
    public AudioMixerGroup SFXAudioMixer;

    [Header("BGM Clips")]
    public AudioClip TitleBackground;
    public AudioClip LevelBackground;

    [Header("SFX Clips")]
    public AudioClip ButtonClick;
    public AudioClip BubbleShootSound;
    public AudioClip BubblePopSound;
    public AudioClip WinSound;
    public AudioClip LoseSound;
    public AudioClip SittingSound;
    public AudioClip GameStartSound;
    public AudioClip GoalSound;
}