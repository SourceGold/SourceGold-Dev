using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectableItem : InteractableObject
{
    public float fullVisibleRange = -1f;
    public float initialVisibleRange = -1f;

    public List<int> count;
    
    private SceneItemActiveUI uiElement;
    private bool activated;
    // Start is called before the first frame update
    void Start()
    {
        uiElement = GetComponentInChildren<SceneItemActiveUI>();
    }

    public override void closestActivation()
    {
        if (uiElement != null) {
            uiElement.setActive(initialVisibleRange, fullVisibleRange); 
        }
    }

    public override void closestDeactivation()
    {
        if (uiElement != null) { 
            uiElement.setInactive(); 
        }
    }
    public override void playerInteract()
    {

    }
    
    public override void inRange() { }
    public override void outRange() { }
}
