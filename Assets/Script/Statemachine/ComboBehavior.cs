using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ComboBehavior : StateMachineBehaviour
{
    public float Start;
    public float End;
    private bool transition = false;


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= Start && stateInfo.normalizedTime < End)
        {
            if (animator.GetBool("IsCombo"))
            {
                animator.SetBool("IsCombo", false);
                transition = true;
            }
        }
        else if (stateInfo.normalizedTime < Start)
        {
            if (animator.GetBool("IsCombo"))
                animator.SetBool("IsCombo", false);
        }
        else
        {
            if (transition)
            {
                transition = false;
                animator.SetTrigger("Combo");
            }
        }
    }
}
