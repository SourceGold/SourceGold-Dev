using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour {
    public float activationRange = -1;
    /// <summary>
    /// The lower the priority, the less likely it is active
    /// Prioirty 0: Less likely to be active:
    /// Priority +infinite: Most likely to be active
    /// 
    /// P0: PickUpAble Objects
    /// </summary>
    public int priority = 0;

    /// <summary>
    /// This function is called whenever the item is within the 
    /// activation range of the object, using the activation range.
    /// It might not be the activated object.
    /// </summary>
    public abstract void inRange();

    /// <summary>
    /// This function is called whenever the item is out of range specified
    /// by the activationRange.
    /// </summary>
    public abstract void outRange();

    /// <summary>
    /// This function is called whenever the item is both in range and cloest
    /// also the priority is highest.
    /// </summary>
    /// <param name="distance">Tell the actual distance with the object</param>
    public abstract void cloestActivation(float distance);

    /// <summary>
    /// This function is called when the object no longer the activated cloest
    /// object.
    /// </summary>
    public abstract void cloestDeactivation();
    /// <summary>
    /// This function is called whenever the player interaction is pressed.
    /// </summary>
    public abstract void playerInteract();

}
