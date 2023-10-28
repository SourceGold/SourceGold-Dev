using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Text;

public class MovementHandler : MonoBehaviour
{
    #region Instance: Camera

    public Camera Camera;
    private Transform _currentLockOnTarget;
    private CameraManager _cameraManager;

    #endregion

    #region Variables: Animation

    private Animator _animator;

    #endregion
    #region Variables: Movement

    private CharacterController _characterController;

    public enum WeaponStatus
    {
        Unequipped,
        Equipped
    }
    [HideInInspector] public WeaponStatus _weaponStatus = WeaponStatus.Unequipped;

    public enum PlayerPosture
    {
        Crouch,
        Stand,
        Falling,
        Jumping,
        Landing,
        LockedOn
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
    private float _lockedHorizontalVelocity;
    private float _lockedVerticalVelocity;

    private float _groundCheckOffset = 0.5f;
    private float _fallHeight = 0.5f;

    private bool _toggleLock = false;
    private bool _isLocked = false;

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
        _isRunning = false;
        _cameraManager = FindObjectOfType<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        LockOnTarget();
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
        _isJumping = context.ReadValueAsButton() && !_animator.GetBool("IsAttacking");
    }

    public void ToggleLockOn(InputAction.CallbackContext context)
    {
        if (context.performed)
            _toggleLock = true; 
    }

    public void ToggleAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _cameraManager.ToggleAim();
        }
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
        }
        else if (_playerPosture == PlayerPosture.Jumping)
        {
            StartCoroutine(CoolDownJump());
        }
        else if (_isLanding)
        {
            _playerPosture = PlayerPosture.Landing;
        }
        else if (_currentLockOnTarget != null)
        {
            _playerPosture = PlayerPosture.LockedOn;
        }
        else
        {
            _playerPosture = PlayerPosture.Stand;
        }
        if (_input.Equals(Vector2.zero))
        {
            _locomotionState = LocomotionState.Idle;
        }
        else if (!_isRunning)
        {
            _locomotionState = LocomotionState.Walking;
        }
        else
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
        else if (_playerPosture == PlayerPosture.LockedOn)
        {
            _animator.SetFloat("Posture", 0.5f, 0.1f, Time.deltaTime);
            MoveLocked();
        }
        else if (_playerPosture == PlayerPosture.Jumping || _playerPosture == PlayerPosture.Falling)
        {
            _animator.SetFloat("Posture", _midAirThreshold);
            _animator.SetFloat("VerticalVelocity", _verticalVelocity);
            if (_playerPosture == PlayerPosture.Jumping)
                _animator.SetFloat("LeftRight", _leftRight);
        }
        

        if (_weaponStatus == WeaponStatus.Equipped)
        {
            _animator.SetFloat("WeaponStatus", 1.0f, 0.1f, Time.deltaTime);
        }
        else
        {
            _animator.SetFloat("WeaponStatus", 0.0f, 0.1f, Time.deltaTime);
        }
    }

    private void LockOnTarget()
    {

        if (_toggleLock && !_isLocked)
        {
            _toggleLock = false;
            _isLocked = true;
            _currentLockOnTarget = _cameraManager.HandleLockOn();
            //_animator.SetBool("lock", true);
        }
        else if (_toggleLock)
        {
            _toggleLock = false;
            _isLocked = false;
            _cameraManager.StopLockOn();
            _currentLockOnTarget = null;
        }

        if (_isLocked && (_currentLockOnTarget == null || !_currentLockOnTarget.gameObject.activeSelf))
        {
            _isLocked = false;
            _cameraManager.StopLockOn();
            _currentLockOnTarget = null;
        }
    }

    private void Rotate()
    {
        if (_playerPosture == PlayerPosture.LockedOn)
        {
            Vector3 dir = _currentLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;
        }
        else if (_input.Equals(Vector2.zero))
            return;
        else
        {
            //_direction.x = _input.x;
            //_direction.z = _input.y;
            Vector3 inputDirection = new Vector3(_input.x, 0.0f, _input.y).normalized;

            var targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, _rotateSpeed);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        }

    }
    private void Move()
    {
        float targetSpeed = _isRunning ? _runSpeed : _walkSpeed;
        targetSpeed *= _input.magnitude;
        _HorizontalVelocity = Mathf.Lerp(_HorizontalVelocity, targetSpeed, 0.5f);
        _animator.SetFloat("HorizontalVelocity", _HorizontalVelocity);
        //var sb = new StringBuilder();
        //sb.Append(_input.x.ToString()).Append("  ").Append(_input.y.ToString());
        //Debug.Log(sb.ToString());
    }
    private void MoveLocked()
    {
        float h, v;
        float targetSpeed = _isRunning ? _runSpeed : _walkSpeed;
        h = _input.x * targetSpeed;
        v = _input.y * targetSpeed;

        if (h != 0 && v != 0)
        {
            h *= 1.5f;
            v *= 1.5f;
        }
        

        _lockedHorizontalVelocity = Mathf.Lerp(_lockedHorizontalVelocity, h, 0.5f);
        _lockedVerticalVelocity = Mathf.Lerp(_lockedVerticalVelocity, v, 0.5f);
        //if (_lockedHorizontalVelocity != 0)
            //_lockedVerticalVelocity = 0;
        _animator.SetFloat("LockedH", _lockedHorizontalVelocity);
        _animator.SetFloat("LockedV", _lockedVerticalVelocity);
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
                if (_weaponStatus == WeaponStatus.Equipped)
                    _leftRight = -1.0f;
                else
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
        if (Physics.SphereCast(transform.position + (Vector3.up * _groundCheckOffset), _characterController.radius, Vector3.down, out RaycastHit hit, _groundCheckOffset - _characterController.radius + 2 * _characterController.skinWidth))
        {
            _isGrounded = true;
        } else
        {
            _isGrounded = false;
            _canFall = !Physics.Raycast(transform.position, Vector3.down, _fallHeight);
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
