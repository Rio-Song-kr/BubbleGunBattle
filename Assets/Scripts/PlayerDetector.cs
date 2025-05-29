using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event Action<bool, Transform> PlayerFound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerFound?.Invoke(true, other.transform);
        }
    }
}