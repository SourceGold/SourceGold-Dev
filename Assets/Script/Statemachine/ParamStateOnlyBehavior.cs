using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParamStateOnlyBehavior : StateMachineBehaviour
{

    public SetParamStateData[] ParamStateData;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(SetParamStateData data in ParamStateData)
        {
            animator.SetBool(data.paramName, data.setDefaultState);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (SetParamStateData data in ParamStateData)
        {
            animator.SetBool(data.paramName, !data.setDefaultState);
        }
    }


    [Serializable]
    public struct SetParamStateData
    {
        public string paramName;
        public bool setDefaultState;
    }
}
