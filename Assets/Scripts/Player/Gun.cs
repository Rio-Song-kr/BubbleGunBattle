using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject _bubblePrefab;
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private float _fireDelay = 1.0f;

    private bool _fire;
    private bool _canFire;
    // private Coroutine _coroutine;

    private void OnEnable()
    {
        _fire = false;
        _canFire = true;
    }

    private void Update()
    {
        if (!_fire || !_canFire) return;

        Instantiate(_bubblePrefab, _fireTransform.position, _fireTransform.rotation);
        _canFire = false;
        StartCoroutine(FireDelay());
    }

    private IEnumerator FireDelay()
    {
        var wait = new WaitForSeconds(_fireDelay);

        yield return wait;
        _canFire = true;
    }

    public void Fire(bool fire)
    {
        _fire = fire;
    }
}