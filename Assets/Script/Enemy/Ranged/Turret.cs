using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : RangedBase
{
    public Transform [] SpawnBulletPosition;
    public float BulletSpeed = 200f;

    private Object pfBulletProjectile;
    private bool IsShooting { get { return LockedTarget != null; } }

    private void Awake()
    {
        pfBulletProjectile = Resources.Load("Prefab/Player/pfSimpleEnemyBulletProjectile");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        PlayerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    protected override void Update()
    {
        Detect();
        base.Update();
    }

    protected override void Shoot()
    {
        if (IsShooting)
        {
            foreach (var spawn in SpawnBulletPosition)
            {
                var bullet = Instantiate(pfBulletProjectile, spawn.position, spawn.rotation).GetComponent<BulletProjectile>();
                bullet.CollisionLayer = PlayerMask;
                bullet.SourceName = transform.name;
                bullet.BulletSpeed = BulletSpeed;
            }
        }
    }
}
