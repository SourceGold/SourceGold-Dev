using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    public Transform TargetTransform;
    public Transform CameraTransform;

    private Transform _nearestLockOnTarget;

    public float MaxLockOnDistance = 30;

    private LockOnCameraManager _lockOnCameraManagerRef;

    List<CharacterManager> availableTargets = new List<CharacterManager>();

    private Animator _anim;

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _lockOnCameraManagerRef = GetComponentInChildren<LockOnCameraManager>();

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
                _nearestLockOnTarget = availableTargets[i].transform;
            }
        }

        if (_nearestLockOnTarget) {
            _anim.SetBool("lock", true);
            _lockOnCameraManagerRef.SetLookAtTarget(_nearestLockOnTarget.Find("Lock On Center"));
        }
            

        return _nearestLockOnTarget;
    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        _nearestLockOnTarget = null;
        _anim.SetBool("lock", false);
    }
}
