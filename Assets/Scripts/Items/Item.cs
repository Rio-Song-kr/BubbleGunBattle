using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IBubbleInteractable, ITransformAdjustable
{
    [SerializeField] private ItemDataSO _itemData;

    private Rigidbody _rigidbody;
    private Collider _collider;

    private Coroutine _destroyCoroutine;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        //# 아이템 생성 시 포획이 되지 않는다면 일정 시간 뒤 파괴할 수 있게 Coroutine 시작
        _destroyCoroutine = StartCoroutine(DestroyItemCoroutine());
    }

    public void TrapInBubble()
    {
        //# Item이 갇혀있어야 하므로 중력을 끄고 물리 법칙 적용을 받지 않기 위해 kinematic true;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;

        _collider.enabled = false;

        //# 포획되어 있을 때는 파괴되지 않아야 하기에 Coroutine 중지
        StopCoroutine(_destroyCoroutine);
    }

    public void PopBubble()
    {
        //# 버블이 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
        _rigidbody.transform.SetParent(null);
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.zero;

        //# x, z 축으로 회전되어 물체가 기울어지지 않도록 회전 보정
        transform.rotation = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f));

        _collider.enabled = true;

        //# Pop이 되었을 때, 다시 일정 시간 뒤 파괴될 수 있게 Coroutine 시작
        _destroyCoroutine = StartCoroutine(DestroyItemCoroutine());
    }

    public void SetPosition(Vector3 position) => transform.position = position;
    public Vector3 GetPosition() => transform.position;
    public void SetLocalPosition(Vector3 position) => transform.localPosition = position;
    public Vector3 GetLocalPosition() => transform.localPosition;
    public void SetLocalScale(Vector3 localScale) => transform.localScale = localScale;
    public void SetParent(Transform parent, bool worldPositionStays = true) => transform.SetParent(parent, worldPositionStays);

    public void DestroySelf()
    {
        DestroyItem();
    }

    private IEnumerator DestroyItemCoroutine()
    {
        yield return _itemData.DestroyDelay;

        DestroyItem();
    }

    private void DestroyItem()
    {
        ItemManager.Instance.RemoveItem(gameObject);
        Destroy(gameObject);
    }
}