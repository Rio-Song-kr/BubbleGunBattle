using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerRotation : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] [Range(1f, 10f)] private float _rotateSpeed;
    [SerializeField] [Range(0.5f, 5f)] private float _pitchSpeed;

    [Header("Pitch Settings")]
    [SerializeField] private bool _pitchInverse;
    [SerializeField] [Range(-60f, 0f)] private float _minPitch;
    [SerializeField] [Range(0f, 60f)] private float _maxPitch;
    [SerializeField] private Transform _followTransform;

    private Player _player;
    private Vector2 _rotateInput;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.Input.OnRotate -= GetRotateInput;
        _player.Input.OnRotate += GetRotateInput;
    }

    private void OnDisable()
    {
        _player.Input.OnRotate -= GetRotateInput;
    }

    private void FixedUpdate()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        float turn = _rotateSpeed * _rotateInput.x;
        _player.Rigid.rotation *= Quaternion.Euler(0f, turn, 0f);

        float currentPitch = _followTransform.localRotation.eulerAngles.x;

        if (currentPitch > 180f) currentPitch -= 360f;

        float pitch = Mathf.Clamp(
            currentPitch + _rotateInput.y * _pitchSpeed * (_pitchInverse ? 1 : -1),
            _minPitch,
            _maxPitch
        );

        _followTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void GetRotateInput(Vector2 rotateInput)
    {
        _rotateInput = rotateInput.normalized;
    }
}