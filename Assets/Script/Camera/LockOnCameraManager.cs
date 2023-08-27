using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LockOnCameraManager : MonoBehaviour
{
    private Transform playerBot;
    private CinemachineFreeLook freeLook;
    // Start is called before the first frame update
    void Start()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        playerBot = GetComponentInParent<CameraManager>().TargetTransform;
        freeLook.Follow = playerBot;
        //freeLook.LookAt = playerBot.Find("Follow Target");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetLookAtTarget(Transform target)
    {
        freeLook.LookAt = target;
    }

}
