using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IBubbleInteractable, ITransformAdjustable
{
    private Rigidbody _rigidbody;

    //# 추후 Collider 부분과 Collider Enable/Disable하는 부분은 자식 오브젝트에서 override
    private BoxCollider _collider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
    }

    public void InBubble()
    {
        //# Item이 갇혀있어야 하므로 중력을 끄고 물리 법칙 적용을 받지 않기 위해 kinematic true;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;

        _collider.enabled = false;
    }

    public void OutBubble()
    {
        //# 버블이 굴러가거나 했을 때, player도 같이 움직여야 하므로 parent에 추가
        _rigidbody.transform.SetParent(null);
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.zero;

        transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.y, 0f));

        _collider.enabled = true;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 GetPosition() => transform.position;

    public void SetLocalPosition(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 GetLocalPosition() => transform.localPosition;

    public void SetLocalScale(Vector3 localScale)
    {
        transform.localScale = localScale;
    }

    public void SetParent(Transform parent, bool worldPositionStays = true)
    {
        transform.SetParent(parent, worldPositionStays);
    }
}