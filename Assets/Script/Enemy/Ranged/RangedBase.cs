using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBase : MonoBehaviour
{
    public Transform Head;
    public float RateOfFire = 10f;
    public float DetectionAngle = 30f;
    public float DetectionRadius = 15f;
    public float ReleaseLockTime = 0.5f;
    protected LayerMask PlayerMask;
    protected Transform LockedTarget;
    public virtual string LockedBodyPart{get; set;}
    protected float DistanceFromTarget;
    protected float NoTargetTime;
    protected float ShootDelay { get { return 1 / RateOfFire; } }
    protected float ShootWait = 0;
    protected bool AllowShoot { get { return ShootWait >= ShootDelay; } }
    protected Collider[] DetectedColliders = new Collider[10];
    // Start is called before the first frame update

    protected virtual void Awake()
    {
        LockedBodyPart = "Follow Target";
    }

    protected virtual void Start()
    {
        ShootWait += ShootDelay;    
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        ShootWait += Time.deltaTime;
        if (AllowShoot)
        {
            ShootWait = 0;
            Shoot();
        }
    }

    protected virtual void Shoot() { }

    protected virtual bool InSight(Transform target)
    {
        Debug.DrawRay(Head.position, target.position - Head.position);
        return Physics.Raycast(Head.position, target.position - Head.position, DetectionRadius, PlayerMask);
    }

    protected virtual void Detect()
    {
        float shortestDistance = Mathf.Infinity;
        int num = Physics.OverlapSphereNonAlloc(Head.position, DetectionRadius, DetectedColliders, PlayerMask);
        bool noTarget = true;
        for (int i = 0; i < num; i++)
        {
            var c = DetectedColliders[i];
            Vector3 lockTargetDirection = c.transform.position - Head.position;
            DistanceFromTarget = Vector3.Distance(transform.position, c.transform.position);
            float targetAngle = Vector3.Angle(lockTargetDirection, transform.forward);
            if (/*InSight(c.transform) &&*/ shortestDistance > DistanceFromTarget && targetAngle < DetectionAngle && targetAngle > -DetectionAngle)
            {
                shortestDistance = DistanceFromTarget;
                LockedTarget = c.transform.Find(LockedBodyPart);
                noTarget = false;
            }
        }
        if (noTarget)
        {
            NoTargetTime += Time.deltaTime;
            if (NoTargetTime > ReleaseLockTime)
                LockedTarget = null;
        }
    }
}
