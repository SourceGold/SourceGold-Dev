using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private bool _isGrounded;
    private float _groundCheckOffset = 0.5f;
    private bool _isJumping;
    private bool _isFalling;
    private Vector3 _direction;
    private Vector3 _averageVelocity;
    private float _speed;
    private float _verticalVelocity;
    private float _targetSpeed;
    private Transform _transform;
    private CharacterController _characterController;
    private float _currentVelocity;
    private float _gravity = -9.8f;

    static readonly int CACHE_SIZE = 3;
    Vector3[] _velocityCache = new Vector3[CACHE_SIZE];
    int _velocityCacheIndex = 0;

    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _walkSpeed = 1.75f;
    [SerializeField] private float _runSpeed = 3.5f;
    [SerializeField] private float _rotateSpeed = 0.05f;
    [SerializeField] private float _jumpPower = 5.0f;
    [SerializeField] private float _gravityMultiplier = 1.0f;

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
        CheckGround();
        ApplyGravity();
        Rotate();
        Move();
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    public void ToggleRunning(InputAction.CallbackContext context)
    {
        _isRunning = context.performed ? !_isRunning : _isRunning;
    }
    private void Rotate()
    {
        if (_input.Equals(Vector2.zero))
            return;

        _direction.x = _input.x;
        _direction.z = _input.y;

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
    }

    public void TriggerJump(InputAction.CallbackContext context)
    {
        _isJumping = context.ReadValueAsButton();
        if (_isJumping ) { 
            _animator.SetTrigger("Jump");
            _averageVelocity = AverageVelocity();
        }
    }
    private void ApplyGravity()
    {
        if (!_isGrounded)
        {
            _verticalVelocity += _gravity * _gravityMultiplier * Time.deltaTime;
            _isFalling = true;
            _animator.SetBool("Falling", true);
        }
        else
        {
            _verticalVelocity = _gravity * _gravityMultiplier * Time.deltaTime;
        }
    }
    private void OnAnimatorMove()
    {
        Vector3 playerDelterMovement = _animator.deltaPosition;
        if (playerDelterMovement.y != 0.0f)
        {
            playerDelterMovement.x = _averageVelocity.x * Time.deltaTime;
            playerDelterMovement.y = playerDelterMovement.y * _jumpPower;
            playerDelterMovement.z = _averageVelocity.z * Time.deltaTime;  
        } else
        {
            playerDelterMovement.y = _verticalVelocity * Time.deltaTime;
            _isFalling = false;
            _animator.SetBool("Falling", false);
        }

        CacheVelocity(_animator.velocity);
        _characterController.Move(playerDelterMovement);
    }
    private Vector3 AverageVelocity() 
    {
        Vector3 average = Vector3.zero;
        foreach (Vector3 vel in _velocityCache)
        {
            average += vel;
        }

        return average / CACHE_SIZE;
    }
    private void CacheVelocity(Vector3 velocity)
    {
        _velocityCache[_velocityCacheIndex] = velocity;
        _velocityCacheIndex = (_velocityCacheIndex + 1) % CACHE_SIZE;
    }
    private void CheckGround()
    {
        if (Physics.SphereCast(_transform.position + (Vector3.up * _groundCheckOffset), _characterController.radius, Vector3.down, out RaycastHit hit, _groundCheckOffset - _characterController.radius + 2 * _characterController.skinWidth))
        {
            _isGrounded = true;
            _animator.SetBool("Grounding", true);
        } else
        {
            _isGrounded = false;
            _animator.SetBool("Grounding", false);
        }
    }
}
