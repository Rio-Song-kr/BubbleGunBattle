using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float _playerMoveSpeed;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance;

    private Player _player;
    private Vector2 _moveInput;
    private float _smoothInputX;
    private float _smoothInputZ;
    private bool _isFirstZero = true;

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
        //# velocity로 Player를 움직일 때, 경사나 턱이 있을 경우, 날아가는 현상을 막기 위해 Ground Check
        //# 플레이어의 기준 축이 바닥이기에 offset을 추가하여 origin 보정
        bool isGrounded =
            Physics.Raycast(transform.position + Vector3.up, Vector3.down, _groundCheckDistance, _groundLayer);

        var moveDirection = (_moveInput.y * transform.forward + _moveInput.x * transform.right).normalized;
        moveDirection *= _playerMoveSpeed;

        //# 땅에 붙어서 움직일 때와 공중에서 떨어지는 상황에서 가하는 힘의 보정을 위한 변수
        float forceMultiply;

        if (isGrounded)
        {
            moveDirection.y = 0f;
            forceMultiply = 5f;
        }
        else
        {
            //# forceMultiply의 값이 크면 떨어지는 도중에 벽 등에 부딪히면 공중에서 멈추는 현상이 있음
            //# 가해주는 힘을 감소시켜 벽 쪽으로 힘을 주더라도 계속 떨어질 수 있게 보정
            forceMultiply = 1.5f;
        }

        _player.Rigid.AddForce(forceMultiply * Time.fixedDeltaTime * moveDirection, ForceMode.VelocityChange);

        if (_moveInput == Vector2.zero)
        {
            //# 정지 시 바로 멈추는 것이 아니라, 조금 부드럽게 정지하기 위함
            if (_isFirstZero)
            {
                _smoothInputX = 1f;
                _smoothInputZ = 1f;
                _isFirstZero = false;
            }
            float yVelocity = isGrounded ? 0f : Mathf.Clamp(_player.Rigid.velocity.y, float.MinValue, 3f);

            _smoothInputX = Mathf.MoveTowards(_smoothInputX, 0f, Time.deltaTime * 5f);
            _smoothInputZ = Mathf.MoveTowards(_smoothInputZ, 0f, Time.deltaTime * 5f);
            _player.Rigid.velocity = new Vector3(_smoothInputX, yVelocity, _smoothInputZ);
        }
        else
        {
            _isFirstZero = true;
            _player.Rigid.velocity = new Vector3(
                Mathf.Clamp(_player.Rigid.velocity.x, -_playerMoveSpeed, _playerMoveSpeed),
                Mathf.Clamp(_player.Rigid.velocity.y, float.MinValue, 3f),
                Mathf.Clamp(_player.Rigid.velocity.z, -_playerMoveSpeed, _playerMoveSpeed)
            );
        }

        //# Velocity의 값을 실제 0 ~ 1 사이로 normalize
        var move = transform.InverseTransformDirection(_player.Rigid.velocity) / _playerMoveSpeed;

        _player.Ani.SetMoveState(move.x, move.z);
    }

    private void GetMoveInput(Vector2 moveInput) => _moveInput = moveInput;
}