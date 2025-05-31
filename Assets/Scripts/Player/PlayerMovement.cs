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
        //# velocity로 Player를 움직일 때, 경사나 턱이 있을 경우, 날아가는 현상을 막기 위해 Ground Check
        bool isGrounded =
            Physics.Raycast(transform.position + new Vector3(0, 1f, 0f), Vector3.down, _groundCheckDistance, _groundLayer);

        var moveDirection = (_moveInput.y * transform.forward + _moveInput.x * transform.right).normalized;
        moveDirection *= _playerMoveSpeed;

        if (isGrounded)
        {
            moveDirection.y = 0f;
        }

        var moveOffset = moveDirection * Time.fixedDeltaTime;

        // //# 충돌 검사
        // if (_player.Rigid.SweepTest(moveOffset.normalized, out var hit, moveOffset.magnitude))
        // {
        //     //# 벽과 충돌했다면 슬라이딩
        //     if (hit.collider.CompareTag("Background"))
        //     {
        //         //# 충돌한 경우, 슬라이딩을 위해 벽에 수직한 방향 제거
        //         var slidingDirection = Vector3.ProjectOnPlane(moveOffset, hit.normal).normalized;
        //
        //         //# 슬라이딩 방향도 검사 후 막혔다면 이동하지 않음
        //         if (_player.Rigid.SweepTest(slidingDirection, out var slideHit, moveOffset.magnitude)) return;
        //
        //         //# 충돌 안 했으면 슬라이딩 방향으로만 이동
        //         _player.Rigid.MovePosition(_player.Rigid.position + slidingDirection * moveOffset.magnitude);
        //
        //         return;
        //     }
        // }
        // //# 충돌을 안했거나 벽이 아니라면 원래 방향으로 이동
        // _player.Rigid.MovePosition(_player.Rigid.position + moveOffset);

        _player.Rigid.AddForce(moveOffset * 10, ForceMode.VelocityChange);

        if (_moveInput == Vector2.zero)
        {
            float yVelocity = 0f;
            if (!isGrounded)
                yVelocity = Mathf.Clamp(_player.Rigid.velocity.y, float.MinValue, 2.5f);
            _player.Rigid.velocity = new Vector3(0f, yVelocity, 0f);
        }
        else
        {
            _player.Rigid.velocity = new Vector3(
                Mathf.Clamp(_player.Rigid.velocity.x, -_playerMoveSpeed, _playerMoveSpeed),
                Mathf.Clamp(_player.Rigid.velocity.y, float.MinValue, 2.5f),
                Mathf.Clamp(_player.Rigid.velocity.z, -_playerMoveSpeed, _playerMoveSpeed)
            );
        }
    }

    private void GetMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }
}