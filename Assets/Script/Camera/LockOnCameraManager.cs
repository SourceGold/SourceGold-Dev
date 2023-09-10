using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LockOnCameraManager : MonoBehaviour
{
    public CinemachineFreeLook FreeLook { get; set; }

    private Transform playerBot;
    // Start is called before the first frame update
    void Start()
    {
        FreeLook = GetComponent<CinemachineFreeLook>();
        playerBot = GetComponentInParent<CameraManager>().Player.Find("Player Bot");

        FreeLook.Follow = playerBot;
    }

    // Update is called once per frame
    void Update()
    {

    }


}
