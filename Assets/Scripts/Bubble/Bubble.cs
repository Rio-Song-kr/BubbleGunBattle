using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private BubbleDataSO _bubbleData;

    private Rigidbody _rigidbody;
    private SphereCollider _collider;

    private ITransformAdjustable _objectTransform;
    private IBubbleInteractable _objectInteractable;

    private Coroutine _growBubbleCoroutine;
    private Coroutine _releaseBubbleCoroutine;

    private bool _isItem;
    private bool _hasObject;
    private Vector3 _objectOffset;
    private float _defaultDrag;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _defaultDrag = _rigidbody.drag;
    }

    //# Disable 시 InitFields가 호출되지만 확실하게 하기 위해서 Enable 시에도 InitFields 호출
    private void OnEnable() => InitFields();

    private void OnDisable()
    {
        var effect = BubblePopEffectPool.Instance.Pool.Get();
        effect.transform.SetPositionAndRotation(transform.position, transform.rotation);
        effect.Play();
    }

    public void Release()
    {
        if (_objectInteractable != null) _objectInteractable.DestroySelf();

        InitFields();

        BubblePool.Instance.Pool.Release(this);
    }

    public void Shoot()
    {
        //# 시작 크기를 BubbleDataSO의 StartScale로 설정
        transform.localScale = Vector3.one * _bubbleData.StartScale;

        ApplyForce(transform.forward * _bubbleData.BubbleShootForce, ForceMode.Impulse);

        //# 총에서 발사한 후 일정 크기까지 크기를 키우기 위한 코루틴
        _growBubbleCoroutine = StartCoroutine(GrowBubble(_bubbleData.EndScale, _bubbleData.GrowDuration));

        //# 일정 시간 후 자동 파괴를 위한 코루틴
        _releaseBubbleCoroutine = StartCoroutine(ReleaseBubble(_bubbleData.UnTrappedReleaseDelay));
    }

    private void FixedUpdate()
    {
        if (_objectTransform == null) return;

        ApplyPositionOffsetToObject();
    }

    private void InitFields()
    {
        _hasObject = _isItem = false;
        _growBubbleCoroutine = _releaseBubbleCoroutine = null;
        gameObject.tag = "Untagged";

        _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = false;
        _rigidbody.drag = _defaultDrag;
        _collider.material = null;

        _objectTransform = null;
        _objectInteractable = null;
        _objectOffset = default;
    }

    private void ApplyForce(Vector3 force, ForceMode mode) => _rigidbody.AddForce(force, mode);

    private void TrapObject(GameObject trappedObject, bool isItem)
    {
        //# 기존 endScale off
        StopCoroutine(_growBubbleCoroutine);
        StopCoroutine(_releaseBubbleCoroutine);
        _growBubbleCoroutine = _releaseBubbleCoroutine = null;

        _isItem = isItem;
        _objectOffset = new Vector3(0f, _isItem ? _bubbleData.ItemYOffset : _bubbleData.PlayerYOffset, 0f);

        _hasObject = true;

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

        //# 일정 시간 후 자동 release를 위한 코루틴
        float bubbleScale;
        WaitForSeconds releaseDelay;
        if (isItem)
        {
            _rigidbody.useGravity = true;
            bubbleScale = _bubbleData.ItemEncapsulatedScale;
            releaseDelay = _bubbleData.ItemTrappedReleaseDelay;
        }
        else
        {
            bubbleScale = _bubbleData.PlayerEncapsulatedScale;
            releaseDelay = _bubbleData.PlayerTrappedReleaseDelay;
        }

        _growBubbleCoroutine = StartCoroutine(GrowBubble(bubbleScale, _bubbleData.GrowDuration));
        _releaseBubbleCoroutine = StartCoroutine(ReleaseBubble(releaseDelay));
    }

    private void SetObjectInBubble()
    {
        //# Object가 Bubble에 갇혔을 때 설정할 메서드 호출
        _objectInteractable.TrapInBubble();

        //# player가 버블에 갇혀 살짝 떠오르는 효과를 위해 offset 적용
        ApplyPositionOffsetToObject();

        //# Object가 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
        _objectTransform.SetParent(transform);
    }

    private void ApplyPositionOffsetToObject() => _objectTransform.SetPosition(transform.position + _objectOffset);

    private IEnumerator ReleaseBubble(WaitForSeconds releaseDelay)
    {
        yield return releaseDelay;

        //# 아이템을 꺼냄
        if (_objectInteractable != null)
        {
            _objectInteractable.PopBubble();

            //# Disable 시 null이 아니라면 Destroy를 하므로, null을 할당해야 함
            //# 버블이 pop 되는 것과 goal에 의해 release 되는 것을 구분하기 위함
            _objectTransform = null;
            _objectInteractable = null;
        }

        Release();
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
                _objectTransform.SetLocalScale(Vector3.one / parentScale);

            time += Time.deltaTime;
            prevScale = parentScale;

            yield return null;
        }

        transform.localScale = Vector3.one * endScale;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_objectInteractable != null || _objectTransform != null) return;

        if (other.gameObject.CompareTag("Player")) TrapObject(other.gameObject, false);
        else if (other.gameObject.CompareTag("Item"))
        {
            //# Bubble 내에 Item이 있을 경우, Goal의 Trigger 감지와 이동을 위한 설정
            gameObject.tag = "Bubble";
            _collider.material = _bubbleData.RollingMaterial;
            TrapObject(other.gameObject, true);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!other.gameObject.CompareTag("Player") || !_hasObject) return;

        var objectDirection = (transform.position - other.gameObject.transform.position).normalized;
        objectDirection.y = Mathf.Clamp(_rigidbody.velocity.y, -0.05f, 0.05f);

        ApplyForce(objectDirection * _bubbleData.ObjectPushForce, ForceMode.Force);

        //# CollisionStay 동안은 release Coroutine 중지
        if (_isItem && _releaseBubbleCoroutine != null)
        {
            StopCoroutine(_releaseBubbleCoroutine);
            _releaseBubbleCoroutine = null;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        _releaseBubbleCoroutine = StartCoroutine(ReleaseBubble(_bubbleData.ItemTrappedReleaseDelay));
    }
}