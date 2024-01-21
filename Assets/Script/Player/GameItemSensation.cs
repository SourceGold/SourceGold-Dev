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
    [SerializeField]
    float forceActivationRange = 0.35f;
    private InputMap input;
    private float relativeZDistance;
    private Camera MainCamera;
    private void Start()
    {
        input = FindObjectOfType<ControlManager>().InputMap;

        input.Player.SceneInteraction.performed += pickupKeyPress;
        relativeZDistance = transform.localPosition.z;
        MainCamera = FindObjectOfType<CameraManager>().GetComponent<Camera>();
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
        InteractableObject item_should_be_active = null;
        closest = null;
        float minimum_score = float.MaxValue;
        int should_be_active_priority = 0;

        bool force_activation_flag = false;
        int force_activation_priority = 0;
        float force_activation_score = float.MaxValue;

        foreach (GameObject item in inRangeItems)
        {
            InteractableObject in_range_interactable = item.GetComponent<InteractableObject>();

            Vector3 ray_start = transform.position - relativeZDistance * transform.forward;
            Vector3 direction = item.transform.position - ray_start;
            Vector3 camera_view_direction = MainCamera.transform.forward;
        
            float camera_to_object_distance = direction.sqrMagnitude + 0.25f;
            direction = direction.normalized;
            ray_start += 0.1f * direction;

            // check distance with the object
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Default", "Environment");

            // Turn off all activation
            in_range_interactable.closestDeactivation();

            if (Physics.Raycast(ray_start, direction, out hit, camera_to_object_distance, (int)layerMask))
            {
                Debug.DrawRay(ray_start, direction * hit.distance, Color.green, 0.5f);

                // check in range
                if (in_range_interactable.activationRange > 0 && hit.distance > in_range_interactable.activationRange) {
                    in_range_interactable.outRange();
                    continue;
                } else {
                    in_range_interactable.inRange();
                }  

                // check activation
                if (hit.collider.gameObject != item) continue;

                // force activation
                if (hit.distance < forceActivationRange || force_activation_flag) {
                    force_activation_flag = true;

                    if (in_range_interactable.priority < force_activation_priority) continue;
                    if (in_range_interactable.priority > force_activation_priority || hit.distance < force_activation_score)
                    {           
                        force_activation_priority = in_range_interactable.priority;
                        force_activation_score = hit.distance;

                        item_should_be_active = in_range_interactable;
                        closest = item;
                    }
                    continue;
                }

                if (in_range_interactable.priority < should_be_active_priority) continue;
                float cameraDotObject = Vector3.Dot(camera_view_direction, direction);
                if (cameraDotObject < 0.25) continue;

                // float score = (float)(hit.distance * Math.Pow(1 - cameraDotObject, 3.0f));
                float score = 1 - cameraDotObject;
                
                if (in_range_interactable.priority > should_be_active_priority || score < minimum_score)
                {           
                    should_be_active_priority = in_range_interactable.priority;
                    minimum_score = score;

                    item_should_be_active = in_range_interactable;
                    closest = item;
                }
            }
        }
        if (item_should_be_active != null) { item_should_be_active.closestActivation(); }
        Invoke("computeClosestObject", stickyTime);
    }
    public void OnTriggerEnter(Collider other)
    {
        InteractableObject gameItem = other.GetComponent<InteractableObject>();
        if (gameItem != null)
        {
            inRangeItems.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        InteractableObject gameItem = other.GetComponent<InteractableObject>();
        if (gameItem != null)
        {
            gameItem.closestDeactivation();
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
