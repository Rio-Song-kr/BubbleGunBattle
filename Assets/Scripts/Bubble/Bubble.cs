using System;
using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private BubbleDataSO _bubbleData;
    [SerializeField] private ObjectDetector _detector;

    private Rigidbody _rigidbody;
    private SphereCollider _collider;

    private ITransformAdjustable _objectTransform;
    private IBubbleInteractable _objectInteractable;

    private Coroutine _growBubbleCoroutine;
    private Coroutine _destroyBubbleCoroutine;

    private bool _isItem;
    private bool _hasObject;
    private Vector3 _objectOffset;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        _detector.ObjectFound -= TrapObject;
        _detector.ObjectFound += TrapObject;

        //# 시작 크기를 BubbleDataSO의 StartScale로 설정
        transform.localScale = Vector3.one * _bubbleData.StartScale;

        Move();
    }

    private void OnDisable()
    {
        _detector.ObjectFound -= TrapObject;

        if (_objectInteractable != null)
        {
            _objectInteractable.DestroySelf();
        }

        //# 추후 Object Pooling 적용 시 수정될 예정
        var effect = Instantiate(_bubbleData.PopEffect, transform.position, transform.rotation);
        Destroy(effect, _bubbleData.PopEffectDuration);
    }

    private void Start()
    {
        //# 총에서 발사한 후 일정 크기까지 크기를 키우기 위한 코루틴
        _growBubbleCoroutine = StartCoroutine(GrowBubble(_bubbleData.EndScale, _bubbleData.GrowDuration));

        //# 일정 시간 후 자동 파괴를 위한 코루틴
        _destroyBubbleCoroutine = StartCoroutine(DestroyBubble(_bubbleData.UnTrappedDestroyDelay));
    }

    private void FixedUpdate()
    {
        if (_objectTransform == null) return;

        ApplyPositionOffsetToObject();
    }

    private void Move()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.AddForce(transform.forward * _bubbleData.BubbleShootForce, ForceMode.Impulse);
    }

    private void TrapObject(GameObject trappedObject, bool isItem)
    {
        //# 기존 endScale off
        StopCoroutine(_growBubbleCoroutine);
        _growBubbleCoroutine = null;

        StopCoroutine(_destroyBubbleCoroutine);
        _destroyBubbleCoroutine = null;

        _isItem = isItem;
        _objectOffset = new Vector3(0f, _isItem ? _bubbleData.ItemYOffset : _bubbleData.PlayerYOffset, 0f);

        // _collider.enabled = false;
        _hasObject = true;

        _detector.ObjectFound -= TrapObject;

        _objectTransform = trappedObject.GetComponent<ITransformAdjustable>();
        _objectInteractable = trappedObject.GetComponent<IBubbleInteractable>();

        //# 버블이 Object를 감싸기 위한 코드
        //# 버블의 position 중 y축은 기존 위치를 유지하고, 나머지는 object의 position을 사용하여 이동
        var position = _objectTransform.GetPosition();
        position.y = transform.position.y;
        transform.position = position;

        //# 버블을 맞은 플레이어 설정
        SetObjectInBubble();

        //# 포획 시 공기 저항 증가
        _rigidbody.drag *= _bubbleData.Drag;

        //# 일정 시간 후 자동 파괴를 위한 코루틴
        if (isItem)
        {
            _rigidbody.useGravity = true;
            //# 새로운 scale 적용
            _growBubbleCoroutine =
                StartCoroutine(GrowBubble(_bubbleData.ItemEncapsulateScale, _bubbleData.GrowDuration));
            _destroyBubbleCoroutine = StartCoroutine(DestroyBubble(_bubbleData.ItemTrappedDestroyDelay));
        }
        else
        {
            //# 새로운 scale 적용
            _growBubbleCoroutine =
                StartCoroutine(GrowBubble(_bubbleData.PlayerEncapsulateScale, _bubbleData.GrowDuration));
            _destroyBubbleCoroutine = StartCoroutine(DestroyBubble(_bubbleData.TrappedDestroyDelay));
        }
    }

    private void SetObjectInBubble()
    {
        //# Object가 갇혀있어야 하므로 중력을 끄고 물리 법칙 적용을 받지 않기 위해 kinematic true;
        _objectInteractable.TrapInBubble();


        //# player가 버블에 갇혀 살짝 떠오르는 효과를 위해 offset 적용
        ApplyPositionOffsetToObject();

        //# Object가 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
        _objectTransform.SetParent(transform);
    }

    private void ApplyPositionOffsetToObject()
    {
        _objectTransform.SetPosition(transform.position + _objectOffset);
    }

    private IEnumerator DestroyBubble(WaitForSeconds destroyDelay)
    {
        yield return destroyDelay;

        if (_objectInteractable != null)
        {
            _objectInteractable.PopBubble();
            _objectInteractable = null;
            _objectTransform = null;
        }

        Destroy(gameObject);
    }

    private IEnumerator GrowBubble(float endScale, float growDuration)
    {
        float time = 0f;
        float prevScale = 0f;

        while (time < growDuration)
        {
            float parentScale = Mathf.Lerp(transform.localScale.x, endScale, time / growDuration);

            transform.position += new Vector3(0, (parentScale - prevScale) / 20, 0);
            transform.localScale = Vector3.one * parentScale;

            if (_objectTransform != null)
            {
                _objectTransform.SetLocalScale(Vector3.one / parentScale);
            }

            time += Time.deltaTime;
            prevScale = parentScale;

            yield return null;
        }

        transform.localScale = Vector3.one * endScale;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_objectInteractable != null || _objectTransform != null) return;

        if (other.gameObject.CompareTag("Player"))
        {
            TrapObject(other.gameObject, false);
        }
        else if (other.gameObject.CompareTag("Item"))
        {
            gameObject.tag = "Bubble";
            _collider.material = _bubbleData.RollingMaterial;
            TrapObject(other.gameObject, true);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!other.gameObject.CompareTag("Player") || !_hasObject) return;

        var playerDirection = (transform.position - other.gameObject.transform.position).normalized;

        playerDirection.y = Mathf.Clamp(_rigidbody.velocity.y, -0.05f, 0.05f);
        _rigidbody.AddForce(playerDirection * _bubbleData.ObjectPushForce, ForceMode.Force);

        //# 충격을 받으면 시간 재설정
        if (_isItem && _destroyBubbleCoroutine != null)
        {
            StopCoroutine(_destroyBubbleCoroutine);
            _destroyBubbleCoroutine = null;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        _destroyBubbleCoroutine = StartCoroutine(DestroyBubble(_bubbleData.ItemTrappedDestroyDelay));
    }
}