using System;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] private BubbleDataSO _bubbleData;
    public event Action<GameObject, bool> ObjectFound;

    private SphereCollider _collider;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _rigidbody = transform.parent.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectFound?.Invoke(other.gameObject, false);
            _collider.isTrigger = false;
        }
        else if (other.CompareTag("Item"))
        {
            _collider.material = _bubbleData.RollingMaterial;
            ObjectFound?.Invoke(other.gameObject, true);
            _collider.isTrigger = false;
        }
    }
}