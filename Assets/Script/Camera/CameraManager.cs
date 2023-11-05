using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{

    private Transform NearestLockOnTarget;
    private Transform CinemachineCameraTarget;
    private CinemachineVirtualCamera FollowCamera;
    private CinemachineVirtualCamera AimCamera;
    private CinemachineVirtualCamera LockCamera;
    private Transform Crosshair;
    private Transform Player;
    private InputMap input;

    private enum CameraState
    {
        Follow,
        Aim,
        LockOn,
    }

    private CameraState currentState = CameraState.Follow;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    public float MaxLockOnDistance = 30;

    List<CharacterManager> availableTargets = new List<CharacterManager>();

    //private Animator _anim;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    private Vector2 _input;

    private void Awake()
    {
        // reference initialization
        FollowCamera = gameObject.transform.Find("Follow Camera").GetComponent<CinemachineVirtualCamera>();
        LockCamera = gameObject.transform.Find("Lock Camera").GetComponent<CinemachineVirtualCamera>();

        CinemachineCameraTarget = FindObjectOfType<PlayerManager>().transform.Find("Player Bot").Find("Follow Target");


        // Camera property initialization
        FollowCamera.Follow = CinemachineCameraTarget;
        LockCamera.Follow = CinemachineCameraTarget;
        LockCamera.gameObject.SetActive(false);

        
        
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.rotation.eulerAngles.y;
        //_anim = GetComponentInChildren<Animator>();

        // register input action
        input = FindObjectOfType<ControlManager>().InputMap;
        input.Player.Look.performed += Look;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    public void Look(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(transform.position, 1);
    //}

    private void CameraRotation()
    {
        if (currentState == CameraState.LockOn)
        {
            Vector3 dir = NearestLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            targetRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
            CinemachineCameraTarget.rotation = targetRotation;
            _cinemachineTargetYaw = ClampAngle(targetRotation.eulerAngles.y, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(targetRotation.eulerAngles.x, BottomClamp, TopClamp);
        }
        else
        {

            //if there is an input and camera position is not fixed
            if (_input.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime;
                //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;

                _cinemachineTargetYaw += _input.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
            //CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            //    _cinemachineTargetYaw, 0.0f);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public Transform HandleLockOn()
    {
        float shortestDistance = Mathf.Infinity;
        currentState = CameraState.LockOn;
        LockCamera.gameObject.SetActive(true);
        Collider[] colliders = Physics.OverlapSphere(CinemachineCameraTarget.position, 26);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();

            if (character != null)
            {
                Vector3 lockTargetDirection = character.transform.position - CinemachineCameraTarget.position;
                float distanceFromTarget = Vector3.Distance(CinemachineCameraTarget.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, transform.forward);

                if (character.transform.root != CinemachineCameraTarget.transform.root
                    && viewableAngle > -50 && viewableAngle < 50
                    && distanceFromTarget <= MaxLockOnDistance)
                {
                    availableTargets.Add(character);
                }
            }

        }

        for (int i = 0; i < availableTargets.Count; i++)
        {
            float distanceFromTarget = Vector3.Distance(CinemachineCameraTarget.position, availableTargets[i].transform.position);
            if (distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                NearestLockOnTarget = availableTargets[i].transform;
            }
        }


        return NearestLockOnTarget;
    }

    public void StopLockOn()
    {
        currentState = CameraState.Follow;
        availableTargets.Clear();
        NearestLockOnTarget = null;
        //_anim.SetBool("lock", false);
        LockCamera.gameObject.SetActive(false);
    }
}
