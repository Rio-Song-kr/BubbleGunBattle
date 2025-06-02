using UnityEngine;

public class Player : MonoBehaviour, IBubbleInteractable, ITransformAdjustable
{
    public InputManager Input;
    public Rigidbody Rigid;

    public bool IsInBubble = false;

    private CapsuleCollider _collider;

    private void Awake()
    {
        Input = GameManager.Instance.Input;
        Rigid = GetComponent<Rigidbody>();

        _collider = GetComponent<CapsuleCollider>();
    }

    public void TrapInBubble()
    {
        //# 플레이어가 갇혀있어야 하므로 중력을 끄고 물리 법칙 적용을 받지 않기 위해 kinematic true;
        Rigid.velocity = Vector3.zero;
        Rigid.useGravity = false;
        Rigid.isKinematic = true;

        _collider.enabled = false;

        IsInBubble = true;
    }

    public void PopBubble()
    {
        //# 버블이 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
        Rigid.transform.SetParent(null);
        Rigid.useGravity = true;
        Rigid.isKinematic = false;
        Rigid.velocity = Vector3.zero;

        transform.rotation = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f));

        _collider.enabled = true;

        IsInBubble = false;
    }

    public void ReleaseToPool()
    {
    }

    public void SetPosition(Vector3 position) => transform.position = position;
    public Vector3 GetPosition() => transform.position;
    public void SetLocalPosition(Vector3 position) => transform.localPosition = position;
    public Vector3 GetLocalPosition() => transform.localPosition;
    public void SetLocalScale(Vector3 localScale) => transform.localScale = localScale;
    public void SetParent(Transform parent, bool worldPositionStays = true) => transform.SetParent(parent, worldPositionStays);
}