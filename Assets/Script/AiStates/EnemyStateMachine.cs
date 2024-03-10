using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    [Header("States")]
    [SerializeField] private EnemyIdleState _idle;
    public EnemyIdleState Idle
    {
        get { return _idle; }
    }
    [SerializeField] private EnemyChaseState _chase;
    public EnemyChaseState Chase
    {
        get { return _chase; }
    }

    private EnemyManager _enemyManager;
    public EnemyManager EnemyManager
    {
        get { return _enemyManager; }
    }

    [Header("Current State")]
    [SerializeField] EnemyState _currentState;

    private void Awake()
    {
        _idle = ScriptableObject.CreateInstance<EnemyIdleState>();
        _chase = ScriptableObject.CreateInstance<EnemyChaseState>();

        _enemyManager = GetComponent<EnemyManager>();
        _currentState = _idle;
    }

    public void ProcessStateMachine()
    {
        EnemyState nextState = _currentState?.Tick(_enemyManager);

        if (nextState != null)
        {
            _currentState = nextState;
        }
    }
}
