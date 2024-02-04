using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public abstract class LocomotionManager : MonoBehaviour
{
    [HideInInspector] public abstract WeaponStatus WeaponStatus { get; set; }
    [HideInInspector] public abstract PlayerPosture PlayerPosture { get; set; }
    [HideInInspector] public abstract LocomotionState LocomotionState { get; set; }

    protected abstract Transform Transform { get; set; }
    protected abstract Animator Animator { get; set; }
    public abstract CharacterController CharacterController { get; set; }
    [SerializeField] private float _rotateSpeed = 15f;
    [SerializeField] private float _gravityMultiplier = 2.0f;
    [SerializeField] private float _fallingMultiplier = 1.5f;
    [SerializeField] private float _jumpCoolDown = 0.15f;
    [SerializeField] private float _jumpPower = 5.0f;
    [SerializeField] private float _leftRightRunningCoef = 3.5f;
    [SerializeField] private float _leftRightWalkingCoef = 2.0f;
    [SerializeField] private float _walkSpeed = 1.75f;
    [SerializeField] private float _runSpeed = 3.5f;

    protected abstract Transform CurrentLockOnTarget { get; set; }
    protected abstract CameraManager CameraManager { get; set; }

    private float _standThreshold = 0.0f;
    private float _lockedThreshold = 0.33f;
    private float _aimThreashold = 0.66f;
    private float _midAirThreshold = 1.1f;
    private float _gravity = -9.8f;
    private float _currentVelocity;
    private float _groundCheckOffset = 0.5f;
    private float _fallHeight = 0.5f;
    private float _leftRight;
    private float _horizontalVelocity;
    private float _lockedHorizontalVelocity;
    private float _lockedVerticalVelocity;
    private float _aimingHorizontalVelocity;
    private float _aimingVerticalVelocity;
    public abstract float VerticalVelocity { get; set; }

    private bool _isGrounded;
    private bool _canFall;
    private bool _isLanding;
    private bool _isLocked = false;

    protected abstract bool IsRunning { get; set; }
    protected abstract bool IsJumping { get; set; }
    protected abstract bool IsAiming { get; set; }
    protected abstract bool ToggleLock { get; set; }


    protected abstract Vector2 Input { get; set; }

    static readonly int CACHE_SIZE = 3;
    Vector3[] _velocityCache = new Vector3[CACHE_SIZE];
    int _velocityCacheIndex = 0;



    protected void PlayerUpdate()
    {
        CheckGround();
        LockOnTarget();
        SwitchPosture();
        ApplyGravity();
        Jump();
        Rotate();
        SetupAnimator();
    }

    protected virtual void Rotate() { }

    protected void Rotate(Quaternion targetAngle)
    {
        Transform.rotation = Quaternion.Slerp(Transform.rotation, targetAngle, _rotateSpeed / Time.deltaTime);
    }

    protected void Rotate(float targetAngle)
    {
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, _rotateSpeed);

        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }
    private void LockOnTarget()
    {

        if (ToggleLock && !_isLocked)
        {
            ToggleLock = false;
            _isLocked = true;
            CurrentLockOnTarget = CameraManager.HandleLockOn();
        }
        else if (ToggleLock)
        {
            ToggleLock = false;
            _isLocked = false;
            CameraManager.StopLockOn();
            CurrentLockOnTarget = null;
        }

        if (_isLocked && (CurrentLockOnTarget == null || !CurrentLockOnTarget.gameObject.activeSelf))
        {
            _isLocked = false;
            CameraManager.StopLockOn();
            CurrentLockOnTarget = null;
        }
    }

    protected void SwitchPosture()
    {
        if (!_isGrounded)
        {
            if (VerticalVelocity > 0.0f)
            {
                PlayerPosture = PlayerPosture.Jumping;
            }
            else if (PlayerPosture != PlayerPosture.Jumping)
            {
                if (_canFall)
                {
                    PlayerPosture = PlayerPosture.Falling;
                }
            }
        }
        else if (PlayerPosture == PlayerPosture.Jumping)
        {
            StartCoroutine(CoolDownJump());
        }
        else if (_isLanding)
        {
            PlayerPosture = PlayerPosture.Landing;
        }
        else if (CurrentLockOnTarget != null)
        {
            PlayerPosture = PlayerPosture.LockedOn;
        }
        else if (IsAiming)
        {
            PlayerPosture = PlayerPosture.Aiming;
        }
        else
        {
            PlayerPosture = PlayerPosture.Stand;
        }
        if (Input.Equals(Vector2.zero))
        {
            LocomotionState = LocomotionState.Idle;
        }
        else if (!IsRunning)
        {
            LocomotionState = LocomotionState.Walking;
        }
        else
        {
            LocomotionState = LocomotionState.Running;
        }

    }

    protected void ApplyGravity()
    {
        if (PlayerPosture != PlayerPosture.Jumping && PlayerPosture != PlayerPosture.Falling)
        {
            if (!_isGrounded)
            {
                VerticalVelocity += _gravity * _fallingMultiplier * _gravityMultiplier * Time.deltaTime;
            }
            else
            {
                VerticalVelocity = _gravity * _gravityMultiplier * Time.deltaTime;
            }
        }
        else
        {
            if (VerticalVelocity <= 0.0f)
            {
                VerticalVelocity += _gravity * _fallingMultiplier * _gravityMultiplier * Time.deltaTime;
            }
            else
            {
                VerticalVelocity += _gravity * _gravityMultiplier * Time.deltaTime;
            }
        }
    }

    protected void CheckGround()
    {
        if (Physics.SphereCast(Transform.position + (Vector3.up * _groundCheckOffset), CharacterController.radius, Vector3.down, out RaycastHit hit, _groundCheckOffset - CharacterController.radius + 2 * CharacterController.skinWidth))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
            _canFall = !Physics.Raycast(Transform.position, Vector3.down, _fallHeight);
        }
    }

    IEnumerator CoolDownJump()
    {
        _isLanding = true;
        PlayerPosture = PlayerPosture.Landing;
        yield return new WaitForSeconds(_jumpCoolDown);
        _isLanding = false;
    }

    #region Functions: Velocity Cache
    protected Vector3 AverageVelocity()
    {
        Vector3 average = Vector3.zero;
        foreach (Vector3 vel in _velocityCache)
        {
            average += vel;
        }

        return average / CACHE_SIZE;
    }
    protected void CacheVelocity(Vector3 velocity)
    {
        _velocityCache[_velocityCacheIndex] = velocity;
        _velocityCacheIndex = (_velocityCacheIndex + 1) % CACHE_SIZE;
    }
    #endregion

    protected void Jump()
    {
        if ((PlayerPosture == PlayerPosture.Stand) && IsJumping)
        {
            VerticalVelocity = _jumpPower;
            _leftRight = Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);

            // Jump from the back leg
            _leftRight = _leftRight < 0.5f ? 1.0f : -1.0f;
            if (LocomotionState == LocomotionState.Running)
            {
                _leftRight *= _leftRightRunningCoef;
            }
            else if (LocomotionState == LocomotionState.Walking)
            {
                _leftRight *= _leftRightWalkingCoef;
            }
            else
            {
                if (WeaponStatus == WeaponStatus.Equipped)
                    _leftRight = -1.0f;
                else
                    _leftRight = Random.Range(-1.0f, 1.0f);
            }
        }
    }

    private void Move()
    {
        float targetSpeed = IsRunning ? _runSpeed : _walkSpeed;
        targetSpeed *= Input.magnitude;
        _horizontalVelocity = Mathf.Lerp(_horizontalVelocity, targetSpeed, 0.5f);
        Animator.SetFloat("HorizontalVelocity", _horizontalVelocity);
    }

    private void MoveLocked()
    {
        float h, v;
        float targetSpeed = IsRunning ? _runSpeed : _walkSpeed;
        h = Input.x * targetSpeed;
        v = Input.y * targetSpeed;

        if (h != 0 && v != 0)
        {
            h *= (float)System.Math.Sqrt(2);
            v *= (float)System.Math.Sqrt(2);
        }


        _lockedHorizontalVelocity = Mathf.Lerp(_lockedHorizontalVelocity, h, 0.5f);
        _lockedVerticalVelocity = Mathf.Lerp(_lockedVerticalVelocity, v, 0.5f);
        //if (_lockedHorizontalVelocity != 0)
        //_lockedVerticalVelocity = 0;
        Animator.SetFloat("LockedH", _lockedHorizontalVelocity);
        Animator.SetFloat("LockedV", _lockedVerticalVelocity);
    }

    private void MoveAiming()
    {
        float h, v;
        float targetSpeed = IsRunning ? _runSpeed : _walkSpeed;
        h = Input.x * targetSpeed;
        v = Input.y * targetSpeed;

        if (h != 0 && v != 0)
        {
            h *= (float)System.Math.Sqrt(2);
            v *= (float)System.Math.Sqrt(2);
        }


        _aimingHorizontalVelocity = Mathf.Lerp(_aimingHorizontalVelocity, h, 0.5f);
        _aimingVerticalVelocity = Mathf.Lerp(_aimingVerticalVelocity, v, 0.5f);
        Animator.SetFloat("AimH", _aimingHorizontalVelocity);
        Animator.SetFloat("AimV", _aimingVerticalVelocity);
    }

    protected void SetupAnimator()
    {
        if (PlayerPosture == PlayerPosture.Stand || PlayerPosture == PlayerPosture.Landing)
        {
            Animator.SetFloat("Posture", _standThreshold, 0.1f, Time.deltaTime);
            Move();
        }
        else if (PlayerPosture == PlayerPosture.LockedOn)
        {
            Animator.SetFloat("Posture", _lockedThreshold, 0.1f, Time.deltaTime);
            MoveLocked();
        }
        else if (PlayerPosture == PlayerPosture.Aiming)
        {
            Animator.SetFloat("Posture", _aimThreashold, 0.1f, Time.deltaTime);
            MoveAiming();
        }
        else if (PlayerPosture == PlayerPosture.Jumping || PlayerPosture == PlayerPosture.Falling)
        {
            Animator.SetFloat("Posture", _midAirThreshold);
            Animator.SetFloat("VerticalVelocity", VerticalVelocity);
            if (PlayerPosture == PlayerPosture.Jumping)
                Animator.SetFloat("LeftRight", _leftRight);
        }

        if (WeaponStatus == WeaponStatus.Equipped)
        {
            Animator.SetFloat("WeaponStatus", 1.0f, 0.1f, Time.deltaTime);
        }
        else
        {
            Animator.SetFloat("WeaponStatus", 0.0f, 0.1f, Time.deltaTime);
        }
    }
}
