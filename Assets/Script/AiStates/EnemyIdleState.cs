using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/States/Idle")]
public class EnemyIdleState : EnemyState
{
    public override EnemyState Tick(EnemyManager enemyManager)
    {
        if (enemyManager.EnemyLocomotionManager.CurrentTarget != null)
        {
            Debug.Log("WE HAVE A TARGET");

            return enemyManager.EnemyStateMachine.Chase;
        }
        else
        {
            enemyManager.EnemyLocomotionManager.StopMoving();
            enemyManager.EnemyLocomotionManager.Detection();

            return this;
        }
    }
}
