using Assets.Script.Backend;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : InteractableObject
{
    public float fullVisibleRange = -1f;
    public float initialVisibleRange = -1f;

    public Animator animator1;
    private SceneItemActiveUI uiElement;
    private Inventory playerInventory;
    private GameItemDynamic test_item = new GameItemDynamic("Potion", dummy: true);

    // Start is called before the first frame update
    void Start()
    {
        uiElement = GetComponentInChildren<SceneItemActiveUI>();
        uiElement.setText("You have to have 10 health potion. \nPress [F] to start");
        playerInventory = Backend.GameLoop.PlayerInventory;
    }

    public override void closestActivation()
    {
        if (uiElement != null)
        {
            if (playerInventory.checkItem(test_item, 10))
                uiElement.setText("Press [F] to use 10 potion to open door");
            else
                uiElement.setText("You don't have enough potion\nRequired 10");
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
        if (playerInventory.checkItem(test_item, 10))
        {
            playerInventory.RemoveItem(test_item, 10);
            animator1.SetBool("OpenDoor", true);
        }
        return false;
    }

    public override void inRange() { }
    public override void outRange() { }
}
