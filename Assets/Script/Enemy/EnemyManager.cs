using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    EnemyLocomotionManager _enemyLocomotionManager;
    private bool _isPerformingAction;
    public bool IsPerformingAction
    {
        get { return _isPerformingAction; }
        set { _isPerformingAction = value;}
    }

    [Header("AI Settings")]
    [SerializeField] private float _detectionRadius = 20;
    public float DetectionRadius
    {
        get { return _detectionRadius; }
        set { _detectionRadius = value; }
    }

    [SerializeField] private float _minimumDetectionAngle = -50;
    public float MinimumDetectionAngle
    {
        get { return _minimumDetectionAngle; }
        set { _minimumDetectionAngle = value; }
    }

    [SerializeField] private float _maximumDetectionAngle = 50;
    public float MaximumDetectionAngle
    {
        get { return _maximumDetectionAngle; }
        set { _maximumDetectionAngle = value; }
    }

    private void Awake()
    {
        _isPerformingAction = false;
        _enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        HandleCurrentAction();
    }

    private void HandleCurrentAction()
    {
        if (_enemyLocomotionManager.CurrentTarget == null)
        {
            _enemyLocomotionManager.HandleDetection();
        } else
        {
            _enemyLocomotionManager.HandleMoveToTarget();
        }
    }

}
