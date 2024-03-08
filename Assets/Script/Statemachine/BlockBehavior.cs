using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : StateMachineBehaviour
{

    private BoxCollider _shieldCollider;

    private void Awake()
    {
        var shield = FindObjectOfType<PlayerManager>().transform.Find("Weapons/Shields/Shield");
        _shieldCollider = shield.GetComponentInChildren<BoxCollider>();
        _shieldCollider.enabled = false;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _shieldCollider.enabled = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _shieldCollider.enabled = false;
    }
}

