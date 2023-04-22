using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class KD_Controller : MonoBehaviour
{
    #region Instance: Camera

    public Transform _cam;

    #endregion
    #region Variables: Animation

    private Animator _animator;

    #endregion
    #region Variables: Movement

    private Vector2 _input;
    private bool _isRunning;
    private Vector3 _direction;
    private float _speed;
    private float _verticalVelocity;
    private float _targetSpeed;
    private Transform _transform;
    private CharacterController _characterController;
    private float _currentVelocity;
    private float _gravity = -9.8f;

    [SerializeField] private float _moveSpeed = 1.0f;
    // [SerializeField] private float _rotateSpeed = 1000.0f;
    [SerializeField] private float _walkSpeed = 1.5f;
    [SerializeField] private float _runSpeed = 3.5f;
    [SerializeField] private float _rotateSpeed = 0.05f;
    [SerializeField] private float _jumpPower = 5.0f;
    [SerializeField] private float _gravityMultiplier = 1.0f;

    private float _moveSpeedMultiplier;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
        _isRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Move();
        ApplyGravity();
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        Debug.Log(_input);
    }

    public void ToggleRunning(InputAction.CallbackContext context)
    {
        _isRunning = context.performed ? !_isRunning : _isRunning;
        Debug.Log(_isRunning);
    }
    private void Rotate()
    {
        if (_input.Equals(Vector2.zero))
            return;

        _direction.x = _input.x;
        _direction.z = _input.y;

        // Quaternion target = Quaternion.LookRotation(_direction, Vector3.up);
        // _transform.rotation = Quaternion.RotateTowards(_transform.rotation, target, _rotateSpeed * Time.deltaTime);

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
        var angle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _currentVelocity, _rotateSpeed);
        _transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }
    private void Move()
    {
        _targetSpeed = _isRunning ? _runSpeed : _walkSpeed;
        _targetSpeed *= _input.magnitude;
        _speed = Mathf.Lerp(_speed, _targetSpeed, 0.5f);
        _animator.SetFloat("HorizontalSpeed", _speed);

        Vector3 vec = new Vector3(0.0f, _verticalVelocity, 0.0f);
        _characterController.Move(vec * Time.deltaTime);
    }
    public void TriggerJump(InputAction.CallbackContext context)
    {
        if (!IsGrounded()) return;

        _verticalVelocity += _jumpPower;
    }

    private void ApplyGravity()
    {
        if (!IsGrounded())
        {
            _verticalVelocity += _gravity * _gravityMultiplier * Time.deltaTime;
            Debug.Log("Falling...");
        }
        else
            _verticalVelocity = _gravity * _gravityMultiplier * Time.deltaTime;

        _direction.y = _verticalVelocity;
    }

    private bool IsGrounded() => _characterController.isGrounded;
}
