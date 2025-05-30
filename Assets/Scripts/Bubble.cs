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
        if (_objectTransform != null)
        {
            var position = _objectTransform.GetPosition();
            _objectTransform.SetPosition(
                new Vector3(
                    transform.position.x,
                    transform.position.y + _bubbleData.PlayerYOffset,
                    transform.position.z
                ));
        }
    }

    private void Move()
    {
        _rigidbody.AddForce(transform.forward * _bubbleData.Force, ForceMode.Impulse);
    }

    private void TrapObject(GameObject gameObject, bool isItem)
    {
        //# 기존 endScale off
        StopCoroutine(_growBubbleCoroutine);
        StopCoroutine(_destroyBubbleCoroutine);

        _collider.enabled = false;

        _detector.ObjectFound -= TrapObject;

        _objectTransform = gameObject.GetComponent<ITransformAdjustable>();
        _objectInteractable = gameObject.GetComponent<IBubbleInteractable>();

        //# 버블이 Player를 감싸기 위한 코드
        //# 버블의 position 중 y축은 기존 위치를 유지하고, 나머지는 player의 position을 사용하여 이동
        var position = _objectTransform.GetPosition();
        position.y = transform.position.y;
        transform.position = position;

        //# 버블을 맞은 플레이어 설정
        SetObjectInBubble();

        //# 새로운 scale 적용
        _growBubbleCoroutine =
            StartCoroutine(GrowBubble(_bubbleData.EncapsulateScale, _bubbleData.GrowDuration));

        //# 포획 시 공기 저항 증가
        _rigidbody.drag *= _bubbleData.Drag;

        //# 일정 시간 후 자동 파괴를 위한 코루틴
        if (!isItem)
            _destroyBubbleCoroutine = StartCoroutine(DestroyBubble(_bubbleData.TrappedDestroyDelay));
        else
        {
            _rigidbody.useGravity = true;
            _destroyBubbleCoroutine = StartCoroutine(DestroyBubble(_bubbleData.ItemTrappedDestroyDelay));
        }
    }

    private void SetObjectInBubble()
    {
        //# 플레이어가 갖혀있어야 하므로 중력을 끄고 물리 법칙 적용을 받지 않기 위해 kinematic true;
        _objectInteractable.InBubble();

        //# 버블이 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
        _objectTransform.SetParent(transform);

        //# player가 버블에 갖혀 살짝 떠오르는 효과를 위해 offset 적용
        _objectTransform.SetPosition(_objectTransform.GetPosition() + new Vector3(0f, _bubbleData.PlayerYOffset, 0f));
    }

    private IEnumerator DestroyBubble(WaitForSeconds destroyDelay)
    {
        yield return destroyDelay;

        if (_objectInteractable != null)
        {
            _objectInteractable.OutBubble();
            _objectInteractable = null;
            _objectTransform = null;
        }

        //# 추후 Object Pooling 적용 시 수정될 예정
        var effect = Instantiate(_bubbleData.PopEffect, transform.position, transform.rotation);
        Destroy(effect, _bubbleData.PopEffectDuration);
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
        if (!other.gameObject.CompareTag("Player")) return;

        var playerDirection = (transform.position - other.gameObject.transform.position).normalized;

        playerDirection.y = Mathf.Clamp(_rigidbody.velocity.y, -0.05f, 0.05f);
        _rigidbody.AddForce(playerDirection * _bubbleData.Force, ForceMode.Impulse);
    }
}