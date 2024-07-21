using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BossControlManager : MonoBehaviour
{
    static readonly Vector2 STOP_MOVING = Vector2.zero;
    static readonly Vector2 MOVE_FORWARD = Vector2.up;

    private BossEnemyManager _bossEnemyManager;
    private BossMovementHandler _bossMovementHandler;
    private MeleeHandler _meleeHandler;
    private ShootingHandler _shootingHandler;
    private GameItemSensationHandler _gameItemSensationHandler;
    private ItemQuickAccess _quickAccess;
    private NavMeshAgent _navMeshAgent;
    private Transform _transform;
    private Animator _anim;

    [SerializeField] private LayerMask _detectionLayer;
    public LayerMask DetectionLayer
    {
        get { return _detectionLayer; }
        set { _detectionLayer = value; }
    }

    [SerializeField] private CharacterStats _currentTarget;
    public CharacterStats CurrentTarget
    {
        get { return _currentTarget; }
    }

    [SerializeField] private float _distanceFromTarget;
    [SerializeField] private float _stoppingDistance = 5f;
    [SerializeField] private float _battleDistance = 10f;

    private bool _isPerformingAction;
    private bool _meleeWeaponEquiped;

    private void Awake()
    {
        _bossEnemyManager = FindObjectOfType<BossEnemyManager>();
        _bossMovementHandler = _bossEnemyManager.GetComponent<BossMovementHandler>();
        _meleeHandler = _bossEnemyManager.GetComponent<MeleeHandler>();
        _shootingHandler = _bossEnemyManager.GetComponentInChildren<ShootingHandler>();
        _gameItemSensationHandler = _bossEnemyManager.GetComponentInChildren<GameItemSensationHandler>();
        _quickAccess = FindObjectOfType<ItemQuickAccess>();
        _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        _transform = _bossMovementHandler.GetComponentInChildren<Transform>();
        _anim = GetComponent<Animator>();
        _isPerformingAction = false;
        _meleeWeaponEquiped = false;
    }

    void Start()
    {
        _navMeshAgent.enabled = false;
    }

    void OnDestroy()
    {
    }

    private void FixedUpdate()
    {
        HandleCurrentAction();
    }

    private void HandleCurrentAction()
    {
        if (_currentTarget == null)
        {
            HandleDetection();
        }
        else
        {
            HandleMoveToTarget();
            HandleAttackTarget();
        }
    }

    public void HandleDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(_transform.position, _bossEnemyManager.DetectionRadius, _detectionLayer);

        CharacterStats closestTarget = null;
        float closestDistance = _bossEnemyManager.DetectionRadius;

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
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

                        if (viewableAngle > _bossEnemyManager.MinimumDetectionAngle && viewableAngle < _bossEnemyManager.MaximumDetectionAngle)
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

        if (_isPerformingAction)
        {
            GetMoveInput(STOP_MOVING);
            _navMeshAgent.enabled = false;
        }
        else
        {
            if (_distanceFromTarget > _battleDistance)
            {
                GetMoveInput(MOVE_FORWARD);
                StartRunning();
            }
            else if (_distanceFromTarget > _stoppingDistance)
            {
                GetMoveInput(MOVE_FORWARD);
                StopRunning();
            }
            else
            {
                GetMoveInput(STOP_MOVING);
                StopRunning();
            }
        }

        HandleRotateTowardsTarget();

        _navMeshAgent.transform.localPosition = Vector3.zero;
        _navMeshAgent.transform.localRotation = Quaternion.identity;
    }

    private void HandleRotateTowardsTarget()
    {
        if (_isPerformingAction)
        {
            Vector3 targetDirection = _currentTarget.transform.position - _transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            if (targetDirection == Vector3.zero) { targetDirection = _transform.forward; }
            Quaternion targetAngle = Quaternion.LookRotation(targetDirection);
            _bossMovementHandler.Rotate(targetAngle);
        }
        else
        {
            _navMeshAgent.enabled = true;
            _navMeshAgent.SetDestination(_currentTarget.transform.position);
            _bossMovementHandler.Rotate(_navMeshAgent.transform.rotation);
        }
    }

    public void HandleAttackTarget()
    {
        _meleeWeaponEquiped = _anim.GetBool("IsWeaponEquipped");
        if (_distanceFromTarget <= _battleDistance)
        {
            if (!_meleeWeaponEquiped)
            {
                EquipWeapon();
            }

            if (_distanceFromTarget <= _stoppingDistance)
                StandingMeleeLight();
        } 
        else
        {
            if (_meleeWeaponEquiped)
            {
                EquipWeapon();
            }
        }
    }

    #region Movement Bindings
    private void GetMoveInput(Vector2 moveVector)
    {
        _bossMovementHandler.GetMoveInput(moveVector);
        
    }
    private void StartRunning()
    {
        _bossMovementHandler.ToggleRunning(true);
    }

    private void StopRunning()
    {
        _bossMovementHandler.ToggleRunning(false);
    }

    private void TriggerJump()
    {
        _bossMovementHandler.TriggerJump(true);
    }

    private void ToggleLockOn()
    {
        _bossMovementHandler.ToggleLockOn();
    }
    private void TriggerRoll()
    {
        _bossMovementHandler.TriggerRoll();
    }
    private void SwitchBattlePoseMovement()
    {
        _bossMovementHandler.SwitchBattlePose();
    }
    #endregion

    #region Melee Bindings
    private void EquipWeapon()
    {
        _meleeHandler.EquipWeapon();
    }

    private void SwitchWeapon()
    {
        _meleeHandler.SwitchWeapon();
    }

    private void StandingMeleeLight()
    {
        _meleeHandler.StandingMeleeLight();
    }

    private void StandingMeleeHeavy()
    {
        _meleeHandler.StandingMeleeHeavy();
    }

    private void SwitchBattlePoseMelee()
    {
        _meleeHandler.SwitchBattlePose();
    }

    private void AttackRelease()
    {
        _meleeHandler.AttackRelease();
    }
    #endregion

    #region Range Bindings
    private void StartShooting()
    {
        _shootingHandler.HandleShoot(true, false);
    }

    private void StopShooting()
    {
        _shootingHandler.HandleShoot(false, true);
    }

    private void ToggleAim()
    {
        _shootingHandler.ToggleAim();
    }
    #endregion

    #region Interaction Bindings
    private void PickupKeyPress()
    {
        _gameItemSensationHandler.PickupKeyPress();
    }
    #endregion

    #region Quick Access
    private void QuickAccess1OnClick() {
        _quickAccess.OnClicked(0);
    }

    private void QuickAccess2OnClick()
    {
        _quickAccess.OnClicked(1);

    }

    private void QuickAccess3OnClick()
    {
        _quickAccess.OnClicked(2);
    }

    private void QuickAccess4OnClick()
    {
        _quickAccess.OnClicked(3);
    }
    #endregion
}
