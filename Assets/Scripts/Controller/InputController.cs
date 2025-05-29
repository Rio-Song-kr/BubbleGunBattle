using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;

    //# 이벤트
    public event Action<Vector2> OnMove;
    public event Action<bool> OnFire;
    public event Action<Vector2> OnRotate;

    //# 각 Input Action을 참조할 필드
    private InputAction _move;
    private InputAction _fire;
    private InputAction _rotate;

    private void OnEnable()
    {
        //# 활성화된 Action Map이지만, 확실히 하기 위해 활성화
        _move = _inputActions.FindAction("Player/Move");
        _fire = _inputActions.FindAction("Player/Fire");
        _rotate = _inputActions.FindAction("Player/Rotate");

        _move.Enable();
        _fire.Enable();
        _rotate.Enable();

        //# Move 관련 Action 등록 - 움직일 때는 입력값, 입력이 취소될 땐 0
        _move.performed += OnMovePerformed;
        _move.canceled += OnMoveCanceled;

        //# Fire 관련 Action 등록 - 움직일 때는 true, 입력이 취소될 땐 false
        _fire.performed += OnFirePerformed;
        _fire.canceled += OnFireCanceled;

        //# Rotate 관련 Action 등록 - 움직일 때는 입력값, 입력이 취소될 땐 0
        _rotate.performed += OnRotatePerformed;
        _rotate.canceled += OnRotateCanceled;
    }

    private void OnDisable()
    {
        //# 비활성화 시 Disable
        _move.Disable();
        _fire.Disable();
        _rotate.Disable();

        _move.performed -= OnMovePerformed;
        _move.canceled -= OnMoveCanceled;

        _fire.performed -= OnFirePerformed;
        _fire.canceled -= OnFireCanceled;

        _rotate.performed -= OnRotatePerformed;
        _rotate.canceled -= OnRotateCanceled;
    }

    private void Start()
    {
        Debug.Log($"[{Time.frameCount}] InputController Start Called");
    }

    //# 핸들러 메서드
    private void OnMovePerformed(InputAction.CallbackContext ctx) => OnMove?.Invoke(ctx.ReadValue<Vector2>());
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => OnMove?.Invoke(Vector2.zero);

    private void OnFirePerformed(InputAction.CallbackContext ctx) => OnFire?.Invoke(true);
    private void OnFireCanceled(InputAction.CallbackContext ctx) => OnFire?.Invoke(false);

    private void OnRotatePerformed(InputAction.CallbackContext ctx) => OnRotate?.Invoke(ctx.ReadValue<Vector2>());
    private void OnRotateCanceled(InputAction.CallbackContext ctx) => OnRotate?.Invoke(Vector2.zero);
}