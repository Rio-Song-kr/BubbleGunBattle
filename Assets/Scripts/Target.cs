using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int _score;
    private HudPresenter _hudPresenter;

    private void Start()
    {
        var hudGameObject = GameObject.FindWithTag("HUDCanvas");
        _hudPresenter = hudGameObject.GetComponent<HudPresenter>();
        Debug.Log(hudGameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bubble"))
        {
            Destroy(other.gameObject);
            _hudPresenter.IncreaseScore("Player", _score);
        }
    }
}