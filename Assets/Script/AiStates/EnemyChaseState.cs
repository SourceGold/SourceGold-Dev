using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/States/Chase")]
public class EnemyChaseState : EnemyState
{
    public override EnemyState Tick(EnemyManager enemyManager)
    {
        if (enemyManager.EnemyLocomotionManager.IsLostTarget())
        {
            enemyManager.EnemyLocomotionManager.CurrentTarget = null;
            enemyManager.EnemyLocomotionManager.StopMoving();

            return enemyManager.EnemyStateMachine.Idle;
        }

        if (enemyManager.EnemyLocomotionManager.CurrentTarget != null)
        {
            enemyManager.EnemyLocomotionManager.MoveToTarget();

            return this;
        }
        else
        {

            return this;
        }
    }
}
