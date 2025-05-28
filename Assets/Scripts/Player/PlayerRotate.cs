using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [Header("Rotate Settings")]
    [SerializeField] private float _rotateSpeed;

    private Player _player;
    private float _rotateInput;

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
        Rotate();
    }

    private void Rotate()
    {
        float turn = _rotateSpeed * _rotateInput * Time.deltaTime;
        _player.Rigid.rotation *= Quaternion.Euler(0f, turn, 0f);
    }

    private void GetRotateInput(float rotateInput)
    {
        _rotateInput = rotateInput;
    }
}