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
            _collider.isTrigger = false;

            ObjectFound?.Invoke(other.gameObject, false);
        }
        else if (other.CompareTag("Item"))
        {
            gameObject.tag = "Bubble";
            _collider.isTrigger = false;
            ObjectFound?.Invoke(other.gameObject, true);
        }
    }
}