using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int _score;

    private void OnEnable()
    {
        _score = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bubble"))
        {
            Destroy(other.gameObject);
            _score++;
            Debug.Log(_score);
        }
    }
}