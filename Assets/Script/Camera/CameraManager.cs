using Assets.Script.Backend;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private ControlSettings _controlSettings;
    private GraphicsSettings _graphicsSettings;

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

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.001f;
    private const float sensitivityLowLimit = 0.2f;
    private const float sensitivityHighLimit = 20f;
    private readonly float sensitivityLowLog = (float)Math.Log10(sensitivityLowLimit);
    private readonly float sensitivityHighLog = (float)Math.Log10(sensitivityHighLimit);
    private float sensitivity;
    private bool invertCamera;
    private Vector2 _input;

    private void Awake()
    {
        // reference initialization
        FollowCamera = gameObject.transform.Find("Follow Camera").GetComponent<CinemachineVirtualCamera>();
        LockCamera = gameObject.transform.Find("Lock Camera").GetComponent<CinemachineVirtualCamera>();
        AimCamera = gameObject.transform.Find("Aim Camera").GetComponent<CinemachineVirtualCamera>();

        Crosshair = gameObject.transform.Find("Crosshair").Find("crosshair_0");

        CinemachineCameraTarget = FindObjectOfType<PlayerManager>().transform.Find("Follow Target");

        // Camera property initialization
        FollowCamera.Follow = CinemachineCameraTarget;
        LockCamera.Follow = CinemachineCameraTarget;
        AimCamera.Follow = CinemachineCameraTarget;
        LockCamera.gameObject.SetActive(false);
        AimCamera.gameObject.SetActive(false);

        
        EventManager.StartListening(GameEventTypes.SettingsPageChangeEvent, applySetting);
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.rotation.eulerAngles.y;

        _controlSettings = GlobalSettings.globalSettings.userDefinedSettings.Control;
        _graphicsSettings = GlobalSettings.globalSettings.userDefinedSettings.Graphics;
        applySetting();

        input = FindObjectOfType<ControlManager>().InputMap;
        // register input action
        input.Player.Look.started += Look;
        input.Player.Look.performed += Look;
        input.Player.Look.canceled += Look;

        _cinemachineTargetPitch = 12f;
    }

    private void OnDestroy()
    {
        input.Player.Look.started -= Look;
        input.Player.Look.performed -= Look;
        input.Player.Look.canceled -= Look;
    }

    private void Update()
    {
        //Debug.LogFormat("(Yaw: {0}, Pitch: {1})", _cinemachineTargetYaw, _cinemachineTargetPitch);
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

    public void applySetting()
    {
        float sensitivitySetting = _controlSettings.MouseSensitivity;
        sensitivity = (float)Math.Pow(10, sensitivitySetting * (sensitivityHighLog - sensitivityLowLog) + sensitivityLowLog);
        invertCamera = _controlSettings.RevertCameraMovements;

        FollowCamera.m_Lens.FieldOfView = _graphicsSettings.VerticalFov;
    }

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
                if (invertCamera)
                    _input.y = -_input.y;

                _cinemachineTargetYaw += _input.x * deltaTimeMultiplier * sensitivity;
                _cinemachineTargetPitch += _input.y * deltaTimeMultiplier * sensitivity;
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
