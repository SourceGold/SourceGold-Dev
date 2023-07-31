using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{

    public Transform TargetTransform;
    public Transform CameraTransform;
    public Transform NearestLockOnTarget;
    public GameObject CinemachineCameraTarget;
    public CinemachineVirtualCamera AimCamera;
    public Transform Crosshair;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    public float MaxLockOnDistance = 30;

    List<CharacterManager> availableTargets = new List<CharacterManager>();

    private Animator _anim;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    private Vector2 _input;

    private void Awake()
    {
        AimCamera.gameObject.SetActive(false);
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _anim = GetComponentInChildren<Animator>();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    public void ToggleAim()
    {
        if (AimCamera.gameObject.activeSelf)
        {
            AimCamera.gameObject.SetActive(false);
            Crosshair.gameObject.SetActive(false);
        }
        else
        {
            Crosshair.gameObject.SetActive(true);
            AimCamera.gameObject.SetActive(true);
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    private void CameraRotation()
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
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
        //CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
        //    _cinemachineTargetYaw, 0.0f);
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
        Collider[] colliders = Physics.OverlapSphere(TargetTransform.position, 26);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();

            if (character != null)
            {
                Vector3 lockTargetDirection = character.transform.position - TargetTransform.position;
                float distanceFromTarget = Vector3.Distance(TargetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, CameraTransform.forward);

                if (character.transform.root != TargetTransform.transform.root
                    && viewableAngle > -50 && viewableAngle < 50
                    && distanceFromTarget <= MaxLockOnDistance)
                {
                    availableTargets.Add(character);
                }
            }

        }

        for (int i = 0; i < availableTargets.Count; i++)
        {
            float distanceFromTarget = Vector3.Distance(TargetTransform.position, availableTargets[i].lockOnTransForm.position);
            if (distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                NearestLockOnTarget = availableTargets[i].transform;
            }
        }

        if (NearestLockOnTarget)
            _anim.SetBool("lock", true);

        return NearestLockOnTarget;
    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        NearestLockOnTarget = null;
        _anim.SetBool("lock", false);
    }
}
