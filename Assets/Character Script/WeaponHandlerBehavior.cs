using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandlerBehavior : StateMachineBehaviour
{
    public WeaponHandler.Action Action;
    public float EventTime;
    public bool switchWeapon;

    private bool _eventTriggered;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _eventTriggered = false;

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= EventTime && !_eventTriggered)
        {
            _eventTriggered = true;
            animator.GetComponentInParent<WeaponHandler>().ResetWeapon(Action, switchWeapon);
        }
    }
}
