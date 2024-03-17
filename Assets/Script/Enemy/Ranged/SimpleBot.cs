using Assets.Script;
using Assets.Script.Backend;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleBot : RangedBase
{
    public Transform SpawnBulletPosition;
    [Tooltip("Degree per second")]
    public float AimRotationSpeed = 5f;
    public float BulletSpeed = 200f;

    private Object pfBulletProjectile;
    private bool IsShooting { get { return LockedTarget != null; } }
    private float BulletPositionHeight;
    private Transform AlwaysUp;

    private void Awake()
    {
        pfBulletProjectile = Resources.Load("Prefab/Player/pfSimpleEnemyBulletProjectile");
        AlwaysUp = transform.Find("Always Up");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        PlayerMask = LayerMask.GetMask("Player");
        BulletPositionHeight = SpawnBulletPosition.transform.position.y - transform.position.y;
    }

    // Update is called once per frame
    protected override void Update()
    {
        Detect();
        Aim();
        base.Update();
    }

    protected override void Shoot()
    {
        if (IsShooting)
        {
            var bullet = Instantiate(pfBulletProjectile, SpawnBulletPosition.position, transform.rotation).GetComponent<BulletProjectile>();
            bullet.CollisionLayer = PlayerMask;
            bullet.SourceName = transform.name;
            bullet.BulletSpeed = BulletSpeed;
        }
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
        if (LockedTarget)
        {
            Vector3 euler = Quaternion.LookRotation(LockedTarget.position - transform.position).eulerAngles;
            if (DistanceFromTarget > BulletPositionHeight)
            {
                float angle = Calculate(LockedTarget);
                euler.x = angle;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(euler), AimRotationSpeed * Time.deltaTime);
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
