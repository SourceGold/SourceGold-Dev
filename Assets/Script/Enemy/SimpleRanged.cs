using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleRanged : MonoBehaviour
{
    public float RateOfFire = 10f;
    public Transform Head;
    public Transform SpawnBulletPosition;
    public float DetectionAngle = 30f;
    public float DetectionRadius = 15f;
    public float AimRotationSpeed = 5f;
    public float ReleaseLockTime = 0.5f;

    private Object pfBulletProjectile;
    private bool IsShooting { get {return LockedTarget != null; } }
    private float ShootDelay { get { return 1 / RateOfFire; } }
    private float ShootWait = 0;
    private bool AllowShoot { get { return ShootWait >= ShootDelay; } }
    private LayerMask PlayerMask;
    private Transform LockedTarget;
    private float NoTargetTime;

    private void Awake()
    {
        pfBulletProjectile = Resources.Load("Prefab/Player/pfBulletProjectile");
    }

    // Start is called before the first frame update
    void Start()
    {
        ShootWait += ShootDelay;
        PlayerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
        ShootWait += Time.deltaTime;
        if (AllowShoot && IsShooting)
        {
            ShootWait = 0;
            var bullet = Instantiate(pfBulletProjectile, SpawnBulletPosition.position, transform.rotation).GetComponent<BulletProjectile>();
            bullet.collisionLayer = PlayerMask;
            bullet.sourceName = transform.name;
        }
    }

    bool InSight(Transform target)
    {
        Debug.DrawRay(Head.position, target.position - Head.position);
        RaycastHit hit;
        Debug.Log(Physics.Raycast(Head.position, target.position - Head.position, out hit, DetectionRadius));
        Debug.Log(hit.transform);
        return Physics.Raycast(Head.position, target.position - Head.position, DetectionRadius, PlayerMask);
    }

    void Aim()
    {
        float shortestDistance = Mathf.Infinity;
        Collider[] colliders = Physics.OverlapSphere(Head.position, DetectionRadius, PlayerMask);
        bool noTarget = true;
        foreach (var c in colliders)
        {
            Vector3 lockTargetDirection = c.transform.position - Head.position;
            float distanceFromTarget = Vector3.Distance(Head.position, c.transform.position);
            float targetAngle = Vector3.Angle(lockTargetDirection, transform.forward);
            Debug.Log(distanceFromTarget);
            if (/*InSight(c.transform) &&*/ shortestDistance > distanceFromTarget && targetAngle < DetectionAngle && targetAngle > -DetectionAngle)
            {
                shortestDistance = distanceFromTarget;
                LockedTarget = c.transform;
                noTarget = false;
            }
        }
        if (noTarget)
        {
            NoTargetTime += Time.deltaTime;
            if (NoTargetTime > ReleaseLockTime)
                LockedTarget = null;
        }

        if (LockedTarget)
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(LockedTarget.position - transform.position), AimRotationSpeed * Time.deltaTime);
    }
}
