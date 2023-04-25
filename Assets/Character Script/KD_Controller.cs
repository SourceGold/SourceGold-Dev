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

    private Transform _transform;
    private CharacterController _characterController;
    public enum PlayerPosture
    {
        Crouch,
        Stand,
        Falling,
        Jumping,
        Landing
    };
    [HideInInspector] public PlayerPosture _playerPosture = PlayerPosture.Stand;

    public enum LocomotionState
    {
        Idle,
        Walking,
        Running
    };
    [HideInInspector] public LocomotionState _locomotionState = LocomotionState.Idle;

    private float _standThreshold = 0.0f;
    private float _midAirThreshold = 1.1f;

    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _walkSpeed = 1.75f;
    [SerializeField] private float _runSpeed = 3.5f;

    private Vector2 _input;
    private bool _isRunning;
    private bool _isJumping;
    private bool _isGrounded;
    private bool _isLanding;
    private bool _canFall;

    private Vector3 _direction;
    private float _HorizontalVelocity;
    private float _verticalVelocity;
    private float _currentVelocity;
    private float _leftRight;
    private float _distanceToGround;

    private float _groundCheckOffset = 0.5f;
    private float _fallHeight = 0.5f;

    static readonly int CACHE_SIZE = 3;
    Vector3[] _velocityCache = new Vector3[CACHE_SIZE];
    int _velocityCacheIndex = 0;

    private float _gravity = -9.8f;
    [SerializeField] private float _rotateSpeed = 0.05f;
    [SerializeField] private float _jumpPower = 5.0f;
    [SerializeField] private float _gravityMultiplier = 2.0f;
    [SerializeField] private float _fallingMultiplier = 1.5f;
    [SerializeField] private float _leftRightRunningCoef = 3.5f;
    [SerializeField] private float _leftRightWalkingCoef = 2.0f;
    [SerializeField] private float _jumpCoolDown = 0.15f;
    [SerializeField] private bool _runMode = true;
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
        SwitchPosture();
        ApplyGravity();
        Jump();
        Rotate();
        SetupAnimator();
    }
    #region Functions: Inputs
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    public void ToggleRunning(InputAction.CallbackContext context)
    {
        if (_runMode)
            _isRunning = context.performed ? !_isRunning : _isRunning;
        else
            _isRunning = context.ReadValueAsButton();
    }
    public void TriggerJump(InputAction.CallbackContext context)
    {
        _isJumping = context.ReadValueAsButton();
    }
    #endregion
    private void SwitchPosture()
    {
        if (!_isGrounded)
        {
            if (_verticalVelocity > 0.0f)
            {
                _playerPosture = PlayerPosture.Jumping;
            } 
            else if (_playerPosture != PlayerPosture.Jumping)
            {
                if (_canFall)
                {
                    _playerPosture = PlayerPosture.Falling;
                }
            }
        } else if (_playerPosture == PlayerPosture.Jumping)
        {
            StartCoroutine(CoolDownJump());
        } else if (_isLanding)
        {
            _playerPosture = PlayerPosture.Landing;
        }
        else
        {
            _playerPosture = PlayerPosture.Stand;
        }
        if (_input.Equals(Vector2.zero))
        {
            _locomotionState = LocomotionState.Idle;
        } else if (!_isRunning)
        {
            _locomotionState = LocomotionState.Walking;
        } else
        {
            _locomotionState = LocomotionState.Running;
        }

    }
    private void SetupAnimator()
    {
        if (_playerPosture == PlayerPosture.Stand || _playerPosture == PlayerPosture.Landing)
        {
            _animator.SetFloat("Posture", _standThreshold, 0.1f, Time.deltaTime);
            Move();
        }
        else if (_playerPosture == PlayerPosture.Jumping || _playerPosture == PlayerPosture.Falling)
        {
            _animator.SetFloat("Posture", _midAirThreshold);
            _animator.SetFloat("VerticalVelocity", _verticalVelocity);
            if (_playerPosture == PlayerPosture.Jumping)
                _animator.SetFloat("LeftRight", _leftRight);
        }
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
        float targetSpeed = _isRunning ? _runSpeed : _walkSpeed;
        targetSpeed *= _input.magnitude;
        _HorizontalVelocity = Mathf.Lerp(_HorizontalVelocity, targetSpeed, 0.5f);
        _animator.SetFloat("HorizontalVelocity", _HorizontalVelocity);
    }
    private void Jump()
    {
        if ((_playerPosture == PlayerPosture.Stand) && _isJumping)
        {
            _verticalVelocity = _jumpPower;
            _leftRight = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);
            
            // Jump from the back leg
            _leftRight = _leftRight < 0.5f ? 1.0f : -1.0f;
            if (_locomotionState == LocomotionState.Running)
            {
                _leftRight *= _leftRightRunningCoef;
            } else if (_locomotionState == LocomotionState.Walking)
            {
                _leftRight *= _leftRightWalkingCoef;
            } else
            {
                _leftRight = Random.Range(-1.0f, 1.0f);
            }
        }
    }
    IEnumerator CoolDownJump()
    {
        _isLanding = true;
        _playerPosture = PlayerPosture.Landing;
        yield return new WaitForSeconds(_jumpCoolDown);
        _isLanding = false;
    }

    private void ApplyGravity()
    {
        if (_playerPosture != PlayerPosture.Jumping && _playerPosture != PlayerPosture.Falling)
        {
            if (!_isGrounded)
            {
                _verticalVelocity += _gravity * _fallingMultiplier * _gravityMultiplier * Time.deltaTime;
            } else
            {
                _verticalVelocity = _gravity * _gravityMultiplier * Time.deltaTime;
            }
        }
        else
        {
            if (_verticalVelocity <= 0.0f)
            {
                _verticalVelocity += _gravity * _fallingMultiplier * _gravityMultiplier * Time.deltaTime;
            }
            else
            {
                _verticalVelocity += _gravity * _gravityMultiplier * Time.deltaTime;
            }
        }
    }
    #region Functions: Velocity Cache
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
    #endregion
    private void CheckGround()
    {
        if (Physics.SphereCast(_transform.position + (Vector3.up * _groundCheckOffset), _characterController.radius, Vector3.down, out RaycastHit hit, _groundCheckOffset - _characterController.radius + 2 * _characterController.skinWidth))
        {
            _isGrounded = true;
        } else
        {
            _isGrounded = false;
            _canFall = !Physics.Raycast(_transform.position, Vector3.down, _fallHeight);
        }
    }
    private void OnAnimatorMove()
    {
        if (_playerPosture != PlayerPosture.Jumping && _playerPosture != PlayerPosture.Falling)
        {
            Vector3 playerDelterMovement = _animator.deltaPosition;
            playerDelterMovement.y = _verticalVelocity * Time.deltaTime;
            _characterController.Move(playerDelterMovement);
            CacheVelocity(_animator.velocity);
        } else
        {
            Vector3 playerDelterMovement = AverageVelocity();
            playerDelterMovement.y = _verticalVelocity;
            playerDelterMovement *= Time.deltaTime;
            _characterController.Move(playerDelterMovement);
        }
    }
}
