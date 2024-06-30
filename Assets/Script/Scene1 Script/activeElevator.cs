using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activeElevator : InteractableObject
{
    public float fullVisibleRange = -1f;
    public float initialVisibleRange = -1f;

    public Animator animator1;
    public Animator animator2;
    public Animator animator3;
    public Animator animator4;
    public Animator animator5;
    public Animator animator6;
    public Animator animator7;
    public Animator animator8;
    private SceneItemActiveUI uiElement;

    // Start is called before the first frame update
    void Start()
    {
        uiElement = GetComponentInChildren<SceneItemActiveUI>();
        uiElement.setText("Press [F] to start");
    }

    public override void closestActivation()
    {
        if (uiElement != null)
        {
            uiElement.setActive(initialVisibleRange, fullVisibleRange);
        }
    }

    public override void closestDeactivation()
    {
        if (uiElement != null)
        {
            uiElement.setInactive();
        }
    }
    public override bool playerInteract()
    {
        animator1.SetBool("TurnOnElevator", true);
        animator2.SetBool("TurnOnElevator", true);
        animator3.SetBool("TurnOnElevator", true);
        animator4.SetBool("TurnOnElevator", true);
        animator5.SetBool("TurnOnElevator", true);
        animator6.SetBool("TurnOnElevator", true);
        animator7.SetBool("TurnOnElevator", true);
        animator8.SetBool("TurnOnElevator", true);
        return false;
    }

    public override void inRange() { }
    public override void outRange() { }
}
