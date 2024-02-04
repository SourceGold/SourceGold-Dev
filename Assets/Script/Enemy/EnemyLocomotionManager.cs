using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionManager : LocomotionManager
{
    private EnemyManager _enemyManager;
    private EnemyAnimatorManager _enemyAnimatorManager;
    private NavMeshAgent _navMeshAgent;

    private CharacterController _characterController;
    public override CharacterController CharacterController
    {
        get { return _characterController; }
        set { _characterController = value; }
    }

    [SerializeField] private CharacterStats _currentTarget;
    public CharacterStats CurrentTarget 
    { 
        get { return _currentTarget; } 
    }

    [SerializeField] private LayerMask _detectionLayer;
    public LayerMask DetectionLayer
    {
        get { return _detectionLayer; }
        set { _detectionLayer = value; }
    }

    private Transform _transform;
    protected override Transform Transform 
    {
        get { return _transform; }
        set { _transform = value; }
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
    protected override Animator Animator 
    { 
        get { return _enemyAnimatorManager.Animator; }
        set { _enemyAnimatorManager.Animator = value; }
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

    private Vector2 _input;
    protected override Vector2 Input
    {
        get { return _input; }
        set { _input = value; }
    }
    private float _verticalVelocity;
    public override float VerticalVelocity
    {
        get { return _verticalVelocity; }
        set { _verticalVelocity = value; }
    }

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

    private bool _toggleLock;
    protected override bool ToggleLock 
    {
        get { return _toggleLock; }
        set { _toggleLock = value; }
    }

    private float _enemyHorizontalVelocity;

    [SerializeField] private float _distanceFromTarget; 
    [SerializeField] private float _stoppingDistance = 5f;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _enemyManager = GetComponent<EnemyManager>();
        _enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _navMeshAgent.enabled = false;
    }

    public void HandleDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(_transform.position, _enemyManager.DetectionRadius, _detectionLayer);

        CharacterStats closestTarget = null;
        float closestDistance = _enemyManager.DetectionRadius;

        if (colliders.Length > 0 )
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

                if (characterStats != null)
                {
                    Vector3 targetDirection = characterStats.transform.position - _transform.position;
                    float targetDistance = targetDirection.magnitude;

                    if (targetDistance < closestDistance)
                    {
                        closestDistance = targetDistance;
                        float viewableAngle = Vector3.Angle(targetDirection, _transform.forward);

                        if (viewableAngle > _enemyManager.MinimumDetectionAngle && viewableAngle < _enemyManager.MaximumDetectionAngle)
                        {
                            closestTarget = characterStats;
                        }
                    }
                }
            }
        } 

        _currentTarget = closestTarget;
    }

    public void HandleMoveToTarget()
    {
        Vector3 targetDirection = _currentTarget.transform.position - _transform.position;
        _distanceFromTarget = Vector3.Distance(_currentTarget.transform.position, _transform.position);
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (_enemyManager.IsPerformingAction)
        {
            _enemyHorizontalVelocity = Mathf.Lerp(_enemyHorizontalVelocity, 0f, 0.5f);
            _navMeshAgent.enabled = false;
        } 
        else
        {
            if (_distanceFromTarget > _stoppingDistance)
            {
                _enemyHorizontalVelocity = Mathf.Lerp(_enemyHorizontalVelocity, 0.5f, 0.5f);
            }
            else if (_distanceFromTarget <= _stoppingDistance)
            {
                _enemyHorizontalVelocity = Mathf.Lerp(_enemyHorizontalVelocity, 0f, 0.5f);
            }
        }

        _enemyAnimatorManager.Animator.SetFloat("HorizontalVelocity", _enemyHorizontalVelocity);

        HandleRotateTowardsTarget();

        _navMeshAgent.transform.localPosition = Vector3.zero;
        _navMeshAgent.transform.localRotation = Quaternion.identity;

        ApplyGravity();
    }

    private void HandleRotateTowardsTarget()
    {
        if (_enemyManager.IsPerformingAction)
        {
            Vector3 targetDirection = _currentTarget.transform.position - _transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            if (targetDirection == Vector3.zero) { targetDirection = _transform.forward; }
            Quaternion targetAngle = Quaternion.LookRotation(targetDirection);
            Rotate(targetAngle);
        }
        else
        {
            _navMeshAgent.enabled = true;
            _navMeshAgent.SetDestination(_currentTarget.transform.position);
            Rotate(_navMeshAgent.transform.rotation);
        }
    }
}
