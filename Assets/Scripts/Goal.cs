using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private int _score;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bubble"))
        {
            var bubble = other.gameObject.GetComponent<Bubble>();
            bubble.Release();
            GameManager.Instance.AddScore(_score);
        }
    }
}