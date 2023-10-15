using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Text;

public class MovementHandler : LocomotionManager
{
    #region Instance: Camera

    private Transform Cam;
    private Transform _currentLockOnTarget;
    CameraManager _cameraManager;

    #endregion
    #region Variables: Animation

    private Animator _animator;
    protected override Animator Animator 
    {
        get { return _animator; }
        set { _animator = value; }  
    }

    #endregion
    #region Variables: Movement

    private CharacterController _characterController;

    private Vector2 _input;
    protected override Vector2 Input
    {
        get { return _input; }
        set { _input = value; }
    }

    private bool _isRunning;
    protected override bool IsRunning 
    {
        get { return _isRunning; }
        set { _isRunning = value; }
    }
    private bool _isJumping;
    protected override bool IsJumping
    {
        get { return _isJumping; }
        set { _isJumping = value; }
    }

    private float _verticalVelocity;
    public override float VerticalVelocity
    {
        get { return _verticalVelocity; }
        set { _verticalVelocity = value; }
    }

    private Vector3 _direction;

    [SerializeField] private bool _runMode = true;

    private Transform _transform;
    protected override Transform Transform 
    { 
        get { return _transform; } 
        set { _transform = value; } 
    }
    public override CharacterController CharacterController
    {
        get { return _characterController; }
        set { _characterController = value; }
    }

    private WeaponStatus _weaponStatus = WeaponStatus.Unequipped;  
    public override WeaponStatus WeaponStatus 
    {
        get { return _weaponStatus; }
        set { _weaponStatus = value; } 
    }
    private PlayerPosture _playerPosture = PlayerPosture.Stand;
    public override PlayerPosture PlayerPosture 
    {
        get { return _playerPosture; }
        set { _playerPosture = value; }
    }
    private LocomotionState _locomotionState = LocomotionState.Idle;
    public override LocomotionState LocomotionState 
    {
        get { return _locomotionState; }
        set { _locomotionState = value; }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Cam = GetComponentInParent<PlayerManager>().MainCamera;
        //Debug.Log(Cam);
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
        _isRunning = false;
        _cameraManager = FindObjectOfType<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerUpdate();
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

    #endregion

    protected override void Rotate()
    {
        if (_playerPosture == PlayerPosture.LockedOn)
        {
            Vector3 dir = _currentLockOnTarget.position - _transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;
        }
        else if (_input.Equals(Vector2.zero))
            return;
        else
        {
            _direction.x = _input.x;
            _direction.z = _input.y;

            var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
            Rotate(targetAngle);
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MovementHandler : LocomotionManager
{
    #region Instance: Camera

    public Transform _cam;

    #endregion
    #region Variables: Animation

    private Animator _animator;
    protected override Animator Animator 
    {
        get { return _animator; }
        set { _animator = value; }  
    }

    #endregion
    #region Variables: Movement

    private CharacterController _characterController;

    private Vector2 _input;
    protected override Vector2 Input
    {
        get { return _input; }
        set { _input = value; }
    }

    private bool _isRunning;
    protected override bool IsRunning 
    {
        get { return _isRunning; }
        set { _isRunning = value; }
    }
    private bool _isJumping;
    protected override bool IsJumping
    {
        get { return _isJumping; }
        set { _isJumping = value; }
    }

    private float _verticalVelocity;
    public override float VerticalVelocity
    {
        get { return _verticalVelocity; }
        set { _verticalVelocity = value; }
    }

    private Vector3 _direction;

    [SerializeField] private bool _runMode = true;

    private Transform _transform;
    protected override Transform Transform 
    { 
        get { return _transform; } 
        set { _transform = value; } 
    }
    public override CharacterController CharacterController
    {
        get { return _characterController; }
        set { _characterController = value; }
    }

    private WeaponStatus _weaponStatus = WeaponStatus.Unequipped;  
    public override WeaponStatus WeaponStatus 
    {
        get { return _weaponStatus; }
        set { _weaponStatus = value; } 
    }
    private PlayerPosture _playerPosture = PlayerPosture.Stand;
    public override PlayerPosture PlayerPosture 
    {
        get { return _playerPosture; }
        set { _playerPosture = value; }
    }
    private LocomotionState _locomotionState = LocomotionState.Idle;
    public override LocomotionState LocomotionState 
    {
        get { return _locomotionState; }
        set { _locomotionState = value; }
    }
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
        PlayerUpdate();
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
    #endregion

    protected override void Rotate()
    {
        if (_input.Equals(Vector2.zero))
            return;

        _direction.x = _input.x;
        _direction.z = _input.y;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
        Rotate(targetAngle);
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
