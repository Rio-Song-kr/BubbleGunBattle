using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] [Range(1f, 360f)] private float _rotateSpeed;
    [SerializeField] [Range(1f, 360f)] private float _pitchSpeed;

    [Header("Pitch Settings")]
    [SerializeField] private bool _pitchInverse;
    [SerializeField] [Range(-60f, 0f)] private float _minPitch;
    [SerializeField] [Range(0f, 60f)] private float _maxPitch;
    [SerializeField] private Transform _followTransform;

    private Player _player;
    private Vector2 _rotateInput;

    private void Awake()
    {
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
        if (_player.IsInBubble) return;
        UpdateRotation();
    }

    private float _currentPitch;
    private Vector2 _smoothedInput;
    private float _inputSmoothSpeed = 10f;

    private void UpdateRotation()
    {
        _smoothedInput = Vector2.Lerp(_smoothedInput, _rotateInput, _inputSmoothSpeed * Time.deltaTime);

        // Yaw 회전
        float turn = _rotateSpeed * _inputSmoothSpeed * _smoothedInput.x * Time.deltaTime;
        var targetYaw = _player.Rigid.rotation * Quaternion.Euler(0f, turn, 0f);
        // _player.Rigid.rotation = Quaternion.Euler(_player.Rigid.rotation.x, targetYaw.eulerAngles.y, 0f);
        _player.Rigid.rotation = Quaternion.Slerp(_player.transform.rotation, targetYaw, 0.1f);

        // Pitch 회전
        _currentPitch += _smoothedInput.y * _pitchSpeed * (_pitchInverse ? 1 : -1) * Time.deltaTime;
        _currentPitch = Mathf.Clamp(_currentPitch, _minPitch, _maxPitch);

        var targetPitch = Quaternion.Euler(_currentPitch, 0f, 0f);
        _followTransform.localRotation =
            // Quaternion.Slerp(_followTransform.localRotation, targetPitch, Time.deltaTime * _pitchSpeed);
            Quaternion.Slerp(_followTransform.localRotation, targetPitch, 0.1f);
    }

    private void GetRotateInput(Vector2 rotateInput)
    {
        _rotateInput = rotateInput.normalized;
    }
}