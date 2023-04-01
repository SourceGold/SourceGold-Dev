using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BasicMoving : MonoBehaviour
{
    #region Variables: Movement

    private Vector2 _input;
    private CharacterController _characterController;
    private Vector3 _direction;

    [SerializeField] private float _moveSpeed;

    private float _moveSpeedMultiplier;

    #endregion
    #region Variables: Rotation

    [SerializeField] private float smoothTime = 0.05f;
    private float _currentVelocity;

    #endregion
    #region Variables: Gravity

    private float _gravity = -9.81f;
    [SerializeField] private float _gravityMultiplier = 3.0f;
    private float _verticalVelocity;

    #endregion
    #region Variables: Jump
    [SerializeField] private float _jumpPower;
    #endregion
    #region Variables: Animation
    private Animator _animator;
    #endregion

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    private void ApplyGravity()
    {
        if(!IsGrounded())
        {
            _verticalVelocity += _gravity * _gravityMultiplier * Time.deltaTime;
            Debug.Log("isFalling ...");
            _animatorator.SetBool("isFalling", true);
            if (!IsJumping())
            {
                _moveSpeedMultiplier = 0.2f;
            }
        }
        else if (_verticalVelocity < 0.0f)
        {
            _verticalVelocity = -1f;
            Debug.Log("Grounded ...");
            SetMovement();
            _animatorator.SetBool("isFalling", false);
        }

        _direction.y = _verticalVelocity;
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    private void ApplyMovement()
    {
        var vec = new Vector3(0, _direction.y);

        vec.x = _direction.x * _moveSpeed * _moveSpeedMultiplier;
        vec.z = _direction.z * _moveSpeed * _moveSpeedMultiplier;

        //Debug.Log($"x {vec.x}, y {y}, z {vec.z}");
        _characterController.Move(vec * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        //Debug.Log(_input);
        SetMovement();
    }

    public void Combat(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("stop movement");
            _direction.x = 0.0f;
            _direction.z = 0.0f;
            _input.x = 0;
            _input.y = 0;
            _anim.SetBool("run", false);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGrounded()) return;

        _verticalVelocity += _jumpPower;
    }

    private void SetMovement()
    {
        if (IsGrounded())
        {
            _moveSpeedMultiplier = 1.0f;
            _direction = new Vector3(_input.x, 0.0f, _input.y);
            if (_input.x == 0 && _input.y == 0)
                _anim.SetBool("run", false);
            else
                _anim.SetBool("run", true);
        }
    }

    private bool IsGrounded() => _characterController.isGrounded;

    private bool IsJumping() => _animatorator.GetCurrentAnimatorStateInfo(0).IsName("Jump");
}
