using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleRanged : MonoBehaviour
{
    public float RateOfFire = 10f;
    public Transform spawnBulletPosition;
    private Object pfBulletProjectile;

    private bool isShooting = true;
    private float shootDelay { get { return 1 / RateOfFire; } }
    private float shootWait = 0;
    private bool allowShoot { get { return shootWait >= shootDelay; } }
    private LayerMask playerMask;

    private void Awake()
    {
        pfBulletProjectile = Resources.Load("Prefab/Player/pfBulletProjectile");
    }

    // Start is called before the first frame update
    void Start()
    {
        shootWait += shootDelay;
        playerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        shootWait += Time.deltaTime;
        if (allowShoot && isShooting)
        {
            shootWait = 0;
            var bullet = Instantiate(pfBulletProjectile, spawnBulletPosition.position, transform.rotation).GetComponent<BulletProjectile>();
            bullet.collisionLayer = playerMask;
            bullet.sourceName = transform.name;
        }
    }
}
