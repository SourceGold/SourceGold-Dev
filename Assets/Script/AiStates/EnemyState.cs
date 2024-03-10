using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : State
{
    public override State Tick(CharacterManager characterManager)
    {
        return Tick((EnemyManager)characterManager);
    }

    public virtual EnemyState Tick(EnemyManager enemyManager)
    {
        return this;
    }
}
