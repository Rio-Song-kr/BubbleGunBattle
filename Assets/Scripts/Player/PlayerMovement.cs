using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float _playerMoveSpeed;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance;

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
        if (_player.IsInBubble) return;
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        var moveDirection = (_moveInput.y * transform.forward + _moveInput.x * transform.right).normalized;
        moveDirection *= _playerMoveSpeed;
        moveDirection.y = _player.Rigid.velocity.y;

        //# velocity로 Player를 움직일 때, 경사나 턱이 있을 경우, 날아가는 현상을 막기 위해 Ground Check
        bool isGrounded =
            Physics.Raycast(transform.position + new Vector3(0, 1f, 0f), Vector3.down, _groundCheckDistance, _groundLayer);

        if (isGrounded)
        {
            moveDirection.y = 0f;
        }

        //# 속도에 따라 위로 많이 뜨는 현상을 줄이기 위해 1f로 max는 제한
        moveDirection.y = Mathf.Clamp(moveDirection.y, float.MinValue, 1f);

        _player.Rigid.velocity = moveDirection;
    }

    private void GetMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }
}