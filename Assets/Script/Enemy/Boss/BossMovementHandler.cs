using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Assets.Script.Backend;

public class BossMovementHandler : MovementHandler
{
    protected override void Rotate()
    {
        if (PlayerPosture == PlayerPosture.LockedOn)
        {
            Vector3 dir = CurrentLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;
        }
        else if (PlayerPosture == PlayerPosture.Aiming)
        {
            Vector3 dir = ShootingHandler.GetHitPosition() - transform.position;
            dir.Normalize();
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;
        }
        else if (Input.Equals(Vector2.zero))
            return;
        else if (Animator.GetBool("IsRolling"))
            return;
        else if (!Animator.GetCurrentAnimatorStateInfo(3).IsName("Idle") && !Animator.GetCurrentAnimatorStateInfo(3).IsName("AttackIdle") && !Animator.GetBool("IsBlocking") && !Animator.GetCurrentAnimatorStateInfo(3).IsName("AttackSwitchPoseIdle"))
            return;
    }
}
