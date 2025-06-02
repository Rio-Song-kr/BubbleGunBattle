using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private readonly int _hashMoveX = Animator.StringToHash("MoveX");
    private readonly int _hashMoveZ = Animator.StringToHash("MoveZ");

    private void Awake() => _animator = GetComponent<Animator>();

    public void SetMoveState(float moveX, float moveZ)
    {
        _animator.SetFloat(_hashMoveX, moveX);
        _animator.SetFloat(_hashMoveZ, moveZ);
    }
}