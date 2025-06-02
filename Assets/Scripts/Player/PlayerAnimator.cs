using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private readonly int _hashMoveX = Animator.StringToHash("MoveX");
    private readonly int _hashMoveZ = Animator.StringToHash("MoveZ");
    private readonly int _hashIsFalling = Animator.StringToHash("IsFalling");
    private readonly int _hashIsInBubble = Animator.StringToHash("IsInBubble");
    private readonly int _hashFire = Animator.StringToHash("Fire");

    private void Awake() => _animator = GetComponent<Animator>();

    public void SetMoveState(float moveX, float moveZ)
    {
        _animator.SetFloat(_hashMoveX, moveX);
        _animator.SetFloat(_hashMoveZ, moveZ);
    }

    public void SetFallState() => _animator.SetTrigger(_hashIsFalling);
    public void SetBalloonFishState(bool value) => _animator.SetBool(_hashIsInBubble, value);
    public void SetFire() => _animator.SetTrigger(_hashFire);
}