using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private float _fireDelay = 1.0f;

    private bool _fire;
    public bool CanFire { get; private set; }

    private void OnEnable()
    {
        _fire = false;
        CanFire = true;
    }

    private void Update()
    {
        if (!_fire || !CanFire || GameManager.Instance.IsGameOver) return;

        var bubble = BubblePool.Instance.Pool.Get();
        bubble.transform.SetPositionAndRotation(_fireTransform.position, _fireTransform.rotation);
        bubble.Shoot();

        CanFire = false;
        _fire = false;
        StartCoroutine(FireDelay());
    }

    private IEnumerator FireDelay()
    {
        var wait = new WaitForSeconds(_fireDelay);

        yield return wait;
        CanFire = true;
    }

    public void Fire() => _fire = true;
}