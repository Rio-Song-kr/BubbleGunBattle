using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float _force;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Destroy(gameObject, 5f);
        Move();
    }

    private void FixedUpdate()
    {
    }

    private void Move()
    {
        _rigidbody.AddForce(transform.forward * _force, ForceMode.Impulse);
    }
}