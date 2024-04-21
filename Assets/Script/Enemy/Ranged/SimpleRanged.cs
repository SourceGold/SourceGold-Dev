using Assets.Script;
using Assets.Script.Backend;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleRanged : MonoBehaviour
{
    public bool IsTurret;
    public float RateOfFire = 10f;
    public Transform Head;
    public Transform[] SpawnBulletPosition;
    public float DetectionAngle = 30f;
    public float DetectionRadius = 15f;
    [Tooltip("Degree per second")]
    public float AimRotationSpeed = 5f;
    public float ReleaseLockTime = 0.5f;
    public float BulletSpeed = 200f;

    private Object pfBulletProjectile;
    private bool IsShooting { get { return LockedTarget != null; } }
    private float ShootDelay { get { return 1 / RateOfFire; } }
    private float ShootWait = 0;
    private bool AllowShoot { get { return ShootWait >= ShootDelay; } }
    private LayerMask PlayerMask;
    private Transform LockedTarget;
    private float NoTargetTime;
    private float BulletPositionHeight;
    private Transform AlwaysUp;
    private Vector3 CurrentRotateionVelocity;

    private void Awake()
    {
        pfBulletProjectile = Resources.Load("Prefab/Player/pfSimpleEnemyBulletProjectile");
        AlwaysUp = transform.Find("Always Up");
    }

    // Start is called before the first frame update
    void Start()
    {
        ShootWait += ShootDelay;
        PlayerMask = LayerMask.GetMask("Player");
        BulletPositionHeight = SpawnBulletPosition[0].transform.position.y - transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
        ShootWait += Time.deltaTime;
        if (AllowShoot && IsShooting)
        {
            ShootWait = 0;
            foreach (var spawn in SpawnBulletPosition)
            {
                var spawnRotation = IsTurret ? spawn.rotation : transform.rotation;
                var bullet = Instantiate(pfBulletProjectile, spawn.position, spawnRotation).GetComponent<BulletProjectile>();
                bullet.CollisionLayer = PlayerMask;
                bullet.SourceName = transform.name;
                bullet.BulletSpeed = BulletSpeed;
            }
        }
    }

    bool InSight(Transform target)
    {
        Debug.DrawRay(Head.position, target.position - Head.position);
        return Physics.Raycast(Head.position, target.position - Head.position, DetectionRadius, PlayerMask);
    }

    // https://math.stackexchange.com/questions/543496/how-to-find-the-equation-of-a-line-tangent-to-a-circle-that-passes-through-a-g
    float Calculate(Transform target)
    {
        // transform target position to coordinate relative to self position;
        Vector3 targetRelative = AlwaysUp.InverseTransformPoint(target.position);
        targetRelative.Scale(transform.localScale);
        // align x,z to x axis only
        Vector2 targetPosition = new Vector2(Mathf.Sqrt(targetRelative.x * targetRelative.x + targetRelative.z * targetRelative.z), targetRelative.y);
        Vector2 thisPosition = Vector2.zero;
        float distance = Vector2.Distance(thisPosition, targetPosition);
        Vector2 vector = targetPosition - thisPosition;
        float radius = BulletPositionHeight;
        float pho = radius / distance;
        float pho2 = pho * pho;
        Vector2 result = thisPosition + (pho2 * vector) + (pho * Mathf.Sqrt(1 - pho2) * Vector2.Perpendicular(vector));
        return Vector2.Angle(new Vector2(0, radius), result - thisPosition);
    }

    void Aim()
    {
        float shortestDistance = Mathf.Infinity;
        Collider[] colliders = Physics.OverlapSphere(Head.position, DetectionRadius, PlayerMask);
        bool noTarget = true;
        foreach (var c in colliders)
        {
            Vector3 lockTargetDirection = c.transform.position - Head.position;
            float distanceFromTarget = Vector3.Distance(transform.position, c.transform.position);
            float targetAngle = Vector3.Angle(lockTargetDirection, transform.forward);
            if (/*InSight(c.transform) &&*/ shortestDistance > distanceFromTarget && targetAngle < DetectionAngle && targetAngle > -DetectionAngle)
            {
                shortestDistance = distanceFromTarget;
                LockedTarget = c.transform.Find("Follow Target");
                noTarget = false;
            }
        }
        if (noTarget)
        {
            NoTargetTime += Time.deltaTime;
            if (NoTargetTime > ReleaseLockTime)
                LockedTarget = null;
        }

        if (LockedTarget && !IsTurret)
        {
            Debug.Log(LockedTarget);
            Vector3 euler = Quaternion.LookRotation(LockedTarget.position - transform.position).eulerAngles;
            if (shortestDistance > BulletPositionHeight)
            {
                float angle = Calculate(LockedTarget);
                euler.x = angle;
                float time = Mathf.Abs(Vector3.Angle(transform.eulerAngles, euler) / AimRotationSpeed);
                var x = Mathf.SmoothDampAngle(transform.eulerAngles.x, euler.x, ref CurrentRotateionVelocity.x, time);
                var y = Mathf.SmoothDampAngle(transform.eulerAngles.y, euler.y, ref CurrentRotateionVelocity.y, time);
                var z = Mathf.SmoothDampAngle(transform.eulerAngles.z, euler.z, ref CurrentRotateionVelocity.z, time);
                transform.rotation = Quaternion.Euler(x, y, z);
                euler = transform.eulerAngles;
                euler.x = 0;
                AlwaysUp.eulerAngles = euler;
            }
            else
            {
                euler.x = 80;
                transform.eulerAngles = euler;
                euler.x = 0;
                AlwaysUp.eulerAngles = euler;
                for (int i = 0; i < 50; i++)
                    Backend.GameLoop.ProcessDamage(new DamageSource() { SrcObjectName = transform.name, AttackWeapon = WeaponNames.Ranged1 },
                        new DamageTarget() { TgtObjectName = "Player" });
            }
        }
    }
}
