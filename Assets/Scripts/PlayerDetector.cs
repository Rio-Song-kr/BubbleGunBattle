using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event Action<bool, GameObject> PlayerFound;

    private SphereCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerFound?.Invoke(true, other.gameObject);
            _collider.isTrigger = false;
        }
    }
}