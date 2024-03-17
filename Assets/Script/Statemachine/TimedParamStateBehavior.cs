using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimedParamStateBehavior : StateMachineBehaviour
{

    public string paramName;
    public bool setDefaultState;
    public float Start, End;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(paramName, !setDefaultState);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= Start && stateInfo.normalizedTime <= End)
        {
            animator.SetBool(paramName, setDefaultState);
        }
        else
        {
            animator.SetBool(paramName, !setDefaultState);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(paramName, !setDefaultState);
    }
}
