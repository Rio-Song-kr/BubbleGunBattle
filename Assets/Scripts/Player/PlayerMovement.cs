using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float _playerMoveSpeed;

    private Player _player;
    private Vector2 _moveInput;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.Input.OnMove -= GetMoveInput;
        _player.Input.OnMove += GetMoveInput;
    }

    private void OnDisable()
    {
        _player.Input.OnMove -= GetMoveInput;
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        var moveDirection = _moveInput.y * transform.forward + _moveInput.x * transform.right;

        _player.Rigid.MovePosition(_player.Rigid.position + _playerMoveSpeed * Time.deltaTime * moveDirection.normalized);
    }

    private void GetMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }
}