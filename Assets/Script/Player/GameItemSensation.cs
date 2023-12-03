using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Loading;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameItemSensation : MonoBehaviour
{
    
    List<GameObject> inRangeItems = new List<GameObject>();
    GameObject closest;

    [Header("Behavior")]
    [SerializeField]
    float stickyTime;

    private InputMap input;
    private float relativeZDistance;
    private void Start()
    {
        input = FindObjectOfType<ControlManager>().InputMap;

        input.Player.SceneInteraction.performed += pickupKeyPress;
        relativeZDistance = transform.localPosition.z;
    }

    private void Update()
    {
        if (closest == null)
        {
            CancelInvoke();
            computeClosestObject();
        }
    }
    private void computeClosestObject()
    {
        InteractableObject itemShouldBeActivate = null;
        float minDistance = float.MaxValue;
        int currentPriority = 0;
        foreach (GameObject item in inRangeItems)
        {
            InteractableObject inRangeObject = item.GetComponent<InteractableObject>();
            inRangeObject.cloestDeactivation();
            if (inRangeObject.priority < currentPriority) continue;

            Vector3 ray_start = transform.position - relativeZDistance * transform.forward;
            Vector3 direction = item.transform.position - ray_start;
            ray_start += 0.1f * direction;

            float length = direction.sqrMagnitude;
            direction = direction.normalized;

            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Default", "Environment");

            if (Physics.Raycast(ray_start, direction, out hit, length, (int)layerMask))
            {
                Debug.DrawRay(ray_start, direction * hit.distance, Color.green, 0.5f);
                if (inRangeObject.activationRange > 0 && hit.distance > inRangeObject.activationRange) continue;
                print(hit.collider.gameObject);
                if (hit.collider.gameObject != item) continue;
                if (inRangeObject.priority > currentPriority || hit.distance < minDistance)
                {
                    currentPriority = inRangeObject.priority;
                    minDistance = hit.distance;

                    itemShouldBeActivate = inRangeObject;
                    closest = item;
                }
            }
        }
        if (itemShouldBeActivate != null) { itemShouldBeActivate.cloestActivation(minDistance); }
        Invoke("computeClosestObject", stickyTime);
    }
    public void OnTriggerEnter(Collider other)
    {
        InteractableObject gameItem = other.GetComponent<InteractableObject>();
        if (gameItem != null)
        {
            gameItem.inRange();
            inRangeItems.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        InteractableObject gameItem = other.GetComponent<InteractableObject>();
        if (gameItem != null)
        {
            gameItem.cloestDeactivation();
            gameItem.outRange();
            inRangeItems.Remove(other.gameObject);
            closest = null;
        }
    }

    public void pickupKeyPress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (closest != null)
            {
                InteractableObject gameItem = closest.GetComponent<InteractableObject>();
                gameItem.playerInteract();

                // TODO: double check how it is been deleted
                inRangeItems.Remove(closest);
                Destroy(closest);
                closest = null;
            }
        }
    }

}
