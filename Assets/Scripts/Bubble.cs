using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private BubbleDataSO _bubbleData;
    [SerializeField] private PlayerDetector _detector;

    private Rigidbody _rigidbody;
    private SphereCollider _collider;

    private Rigidbody _playerRigidbody;
    private Transform _playerTransform;

    private Coroutine _growBubbleCoroutine;
    private float _parentScale;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        _detector.PlayerFound -= FindPlayer;
        _detector.PlayerFound += FindPlayer;

        transform.localScale = Vector3.one * _bubbleData.StartScale;

        Move();
    }

    private void OnDisable()
    {
        _detector.PlayerFound -= FindPlayer;
    }

    private void Start()
    {
        //# 총에서 발사한 후 일정 크기까지 크기를 키우기 위한 코루틴
        _growBubbleCoroutine = StartCoroutine(GrowBubble(_bubbleData.EndScale, _bubbleData.GrowDuration));

        //# 일정 시간 후 자동 파괴를 위한 코루틴
        StartCoroutine(DestroyBubble());
    }

    private void Move()
    {
        _rigidbody.AddForce(transform.forward * _bubbleData.Force, ForceMode.Impulse);
    }

    private void FindPlayer(bool found, Transform playerTransform)
    {
        if (!found) return;

        _detector.PlayerFound -= FindPlayer;

        _playerTransform = playerTransform;

        //# 기존 endScale off
        StopCoroutine(_growBubbleCoroutine);

        //# 플레이어가 갖혀있어야 하므로 중력을 끄고 물리 법칙 적용을 받지 않기 위해 kinematic true;
        _playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        _playerRigidbody.useGravity = false;
        _playerRigidbody.isKinematic = true;

        //# Player와의 충돌을 방지하기 위해 trigger 설정
        _collider.isTrigger = true;

        //# bubble의 속도 초기화
        _rigidbody.velocity = Vector3.zero;

        //# 버블의 position 중 y축은 기존 위치를 유지하고, 나머지는 player의 position을 사용하여 이동
        var position = _playerTransform.position;
        position.y = transform.position.y;
        transform.position = position;

        //# player가 버블에 갖혀 살짝 떠오르는 효과를 위해 offset 적용
        _playerTransform.position += new Vector3(0f, _bubbleData.PlayerYOffset, 0f);

        //# 버블이 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
        _playerTransform.SetParent(transform);

        var originalScale = _playerTransform.lossyScale;

        _parentScale = transform.localScale.x;

        _playerTransform.localScale = originalScale / _parentScale;

        //# 새로운 scale 적용
        _growBubbleCoroutine = StartCoroutine(GrowBubble(_bubbleData.EncapsulateScale, _bubbleData.GrowDuration));
    }

    private IEnumerator DestroyBubble()
    {
        yield return _bubbleData.DestroyDelay;

        if (_playerRigidbody != null)
        {
            //# 버블이 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
            _playerRigidbody.transform.SetParent(null);
            _playerRigidbody.useGravity = true;
            _playerRigidbody.isKinematic = false;
            _playerRigidbody.velocity = Vector3.zero;
            _playerRigidbody = null;
        }

        //# 추후 Object Pooling 적용 시 수정될 예정
        var effect = Instantiate(_bubbleData.PopEffect, transform.position, transform.rotation);
        Destroy(effect, _bubbleData.PopEffectDuration);
        Destroy(gameObject);
    }

    private IEnumerator GrowBubble(float endScale, float growDuration)
    {
        float time = 0f;

        while (time < growDuration)
        {
            _parentScale = Mathf.Lerp(transform.localScale.x, endScale, time / growDuration);

            transform.localScale = Vector3.one * _parentScale;
            if (_playerTransform != null)
            {
                _playerTransform.localScale = Vector3.one / _parentScale;
                _playerTransform.position = transform.position + new Vector3(0f, _bubbleData.PlayerYOffset, 0f);
            }

            time += Time.deltaTime;

            yield return null;
        }

        transform.localScale = Vector3.one * endScale;
    }
}