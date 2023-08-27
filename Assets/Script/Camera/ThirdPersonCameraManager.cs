using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ThirdPersonCameraManager : MonoBehaviour
{

    private Transform playerBot;
    // Start is called before the first frame update
    void Start()
    {
        var freeLook = GetComponent<CinemachineFreeLook>();
        playerBot = GetComponentInParent<CameraManager>().TargetTransform;
        freeLook.Follow = playerBot;
        freeLook.LookAt = playerBot.Find("Follow Target");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
