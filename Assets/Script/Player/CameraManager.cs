using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Transform TargetTransform;
    public Transform CameraTransform;
    public Transform NearestLockOnTarget;

    public float MaxLockOnDistance = 30;

    List<CharacterManager> availableTargets = new List<CharacterManager>();
    


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

        return NearestLockOnTarget;
    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        NearestLockOnTarget = null;
    }
}
