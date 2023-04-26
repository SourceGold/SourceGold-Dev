using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualTimedParamStateBehavior : StateMachineBehaviour
{
    public string paramName;
    public bool setDefaultState;
    public float Start1, End1, Start2, End2;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(paramName, !setDefaultState);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= Start1 && stateInfo.normalizedTime <= End1)
        {
            animator.SetBool(paramName, setDefaultState);
        }
        else if (stateInfo.normalizedTime >= Start2 && stateInfo.normalizedTime <= End2)
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
