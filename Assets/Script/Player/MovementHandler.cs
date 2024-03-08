using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Assets.Script.Backend;

public class MovementHandler : LocomotionManager
{
    #region Instance: Camera

    private Transform _camT;
    private Camera _cam;
    private Transform _currentLockOnTarget;
    protected override Transform CurrentLockOnTarget 
    {
        get { return _currentLockOnTarget; }
        set { _currentLockOnTarget = value; }
    }
    private CameraManager _cameraManager;
    protected override CameraManager CameraManager
    {  
        get { return _cameraManager; } 
        set { _cameraManager = value; }
    }

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

    private bool _isAiming;
    protected override bool IsAiming
    {
        get { return _isAiming; }
        set { _isAiming = value; }
    }

    private bool _toggleLock;
    protected override bool ToggleLock
    {
        get { return _toggleLock; }
        set { _toggleLock = value; }
    }

    private float _verticalVelocity;
    public override float VerticalVelocity
    {
        get { return _verticalVelocity; }
        set { _verticalVelocity = value; }
    }

    private Vector3 _direction;

    //[SerializeField] private bool _runMode = true;
    //[SerializeField] private bool _runMode = true;

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

    private ShootingHandler _shootingHandler;
    private bool _isBattlePoseSwitched = false;

    // Start is called before the first frame update
    void Awake()
    {
        //Camera = FindObjectOfType<CameraManager>().gameObject.GetComponent<Camera>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _isRunning = false;
        _isAiming = false;
        _cameraManager = FindObjectOfType<CameraManager>();
        _cam = _cameraManager.gameObject.GetComponent<Camera>();
        _camT = _cam.transform;
        _transform = GetComponent<Transform>();
        _shootingHandler = GetComponentInParent<ShootingHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerUpdate();
    }
    #region Functions: Inputs
    public void GetMoveInput(Vector2 moveInput)
    {
        _input = moveInput;
    }

    public void ToggleRunning(InputAction.CallbackContext context)
    {
        if (_weaponStatus == WeaponStatus.Equipped)
            return;
        if (GlobalSettings.globalSettings.userDefinedSettings.Control.PressToSpeedUp)
        {
            _isRunning = context.performed ? !_isRunning : _isRunning;
        }
        else 
        {
            if (context.performed)
                _isRunning = true;
            if (context.canceled)
                _isRunning = false;
        }
    }
    public void TriggerJump(bool performed)
    {
        _isJumping = performed && !_animator.GetBool("IsRolling") && !_animator.GetBool("IsAttacking");
    }

    public void ToggleLockOn()
    {
        if (!_animator.GetBool("IsRolling"))
            _toggleLock = true;
    }

    public void ToggleAim()
    {
        if (!_animator.GetBool("IsRolling") && !_animator.GetBool("RangeStarting") && !_animator.GetBool("IsWeaponReady") && !_animator.GetBool("IsEquipting") && !_shootingHandler.IsMouseLeftDown)
        {
            _cameraManager.ToggleAim();
            _animator.SetBool("IsRangeStart", !_animator.GetBool("IsRangeStart"));
            if (_animator.GetBool("IsRangeStart"))
                IsAiming = true;
            else
                IsAiming = false;
        }
    }

    public void TriggerRoll()
    {
        if (!_animator.GetBool("IsRolling") &&  _playerPosture == PlayerPosture.Stand)
        {
            _animator.SetBool("IsRolling", true);
        }

    }

    public void SwitchBattlePose()
    {
        if (_animator.GetBool("CanAttack") && !_animator.GetBool("IsAttacking"))
        {
            _isBattlePoseSwitched = !_isBattlePoseSwitched;
        }
    }

    #endregion

    public void SetWeaponStatus(bool equip)
    {
        if (equip)
            _weaponStatus = WeaponStatus.Equipped;
        else
            _weaponStatus = WeaponStatus.Unequipped;
    }

    protected override void SetupAnimatorWeapon()
    {
        if (_weaponStatus == WeaponStatus.Equipped && _isBattlePoseSwitched && _animator.GetBool("IsBlocking"))
        {
            _animator.SetFloat("WeaponStatus", 3.0f, 0.1f, Time.deltaTime);
        }
        else if (_weaponStatus == WeaponStatus.Equipped && _isBattlePoseSwitched)
        {
            var weaponStat = (float)(_animator.GetInteger("WeaponType"));
            _animator.SetFloat("WeaponStatus", weaponStat, 0.1f, Time.deltaTime);
        }
        else if (_weaponStatus == WeaponStatus.Equipped)
        {
            Animator.SetFloat("WeaponStatus", 1.0f, 0.1f, Time.deltaTime);
        }
        else
        {
            Animator.SetFloat("WeaponStatus", 0.0f, 0.1f, Time.deltaTime);
        }
    }

    protected override void Rotate()
    {
        if (_playerPosture == PlayerPosture.LockedOn)
        {
            Vector3 dir = _currentLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;
        }
        else if (_playerPosture == PlayerPosture.Aiming)
        {
            Vector3 dir = _shootingHandler.GetHitPosition() - transform.position;
            dir.Normalize();
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;
        }
        else if (_input.Equals(Vector2.zero))
            return;
        else if (_animator.GetBool("IsRolling"))
            return;
        else if (!_animator.GetCurrentAnimatorStateInfo(3).IsName("Idle") && !_animator.GetCurrentAnimatorStateInfo(3).IsName("AttackIdle") && !_animator.GetBool("IsBlocking"))
            return;
        else
        {
            _direction.x = _input.x;
            _direction.z = _input.y;
            Vector3 inputDirection = new Vector3(_input.x, 0.0f, _input.y).normalized;

            var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _camT.eulerAngles.y;
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
        }
        else
        {
            Vector3 playerDelterMovement = AverageVelocity();
            playerDelterMovement.y = _verticalVelocity;
            playerDelterMovement *= Time.deltaTime;
            _characterController.Move(playerDelterMovement);
        }
    }

    public void Teleport(Vector3 DstGloblePosition)
    {
        _characterController.enabled = false;
        transform.position = DstGloblePosition + new Vector3(0, 0.5f, 0);
        _characterController.enabled = true;
    }
}
