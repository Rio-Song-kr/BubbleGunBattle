using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private BubbleDataSO _bubbleData;
    [SerializeField] private PlayerDetector _detector;

    private Rigidbody _rigidbody;
    private SphereCollider _collider;

    private Transform _playerTransform;
    private Player _player;

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

        //# 시작 크기를 BubbleDataSO의 StartScale로 설정
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

        //# 기존 endScale off
        StopCoroutine(_growBubbleCoroutine);

        _detector.PlayerFound -= FindPlayer;

        //# 버블이 Player를 감싸기 위한 코드
        //# 버블의 position 중 y축은 기존 위치를 유지하고, 나머지는 player의 position을 사용하여 이동
        var position = _playerTransform.position;
        position.y = transform.position.y;
        transform.position = position;

        //# 버블을 맞은 플레이어 설정
        SetPlayerInBubble(playerTransform);

        //# 새로운 scale 적용
        _growBubbleCoroutine =
            StartCoroutine(GrowBubble(_bubbleData.EncapsulateScale, _bubbleData.GrowDuration, _playerTransform));
    }

    private void SetPlayerInBubble(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        _player = _playerTransform.GetComponent<Player>();

        //# 플레이어가 갖혀있어야 하므로 중력을 끄고 물리 법칙 적용을 받지 않기 위해 kinematic true;
        _player.InBubble();

        //# player가 버블에 갖혀 살짝 떠오르는 효과를 위해 offset 적용
        _playerTransform.position += new Vector3(0f, _bubbleData.PlayerYOffset, 0f);

        //# 버블이 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
        _playerTransform.SetParent(transform, true);
    }

    private IEnumerator DestroyBubble()
    {
        yield return _bubbleData.DestroyDelay;

        if (_player != null)
        {
            _player.OutBubble();
            _player = null;
        }

        //# 추후 Object Pooling 적용 시 수정될 예정
        var effect = Instantiate(_bubbleData.PopEffect, transform.position, transform.rotation);
        Destroy(effect, _bubbleData.PopEffectDuration);
        Destroy(gameObject);
    }

    private IEnumerator GrowBubble(float endScale, float growDuration, Transform objectTransform = null)
    {
        float time = 0f;

        while (time < growDuration)
        {
            _parentScale = Mathf.Lerp(transform.localScale.x, endScale, time / growDuration);

            transform.position = new Vector3(transform.position.x, _parentScale / 2, transform.position.z);
            transform.localScale = Vector3.one * _parentScale;

            if (objectTransform != null)
            {
                objectTransform.localScale = Vector3.one / _parentScale;
                objectTransform.position = transform.position + new Vector3(0f, _bubbleData.PlayerYOffset, 0f);
            }

            time += Time.deltaTime;

            yield return null;
        }

        transform.localScale = Vector3.one * endScale;
    }
}