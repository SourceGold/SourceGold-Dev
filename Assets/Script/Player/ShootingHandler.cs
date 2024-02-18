using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingHandler : MonoBehaviour
{
    public float RateOfFire = 50f;
    private Camera Camera;
    public LayerMask aimColliderLayerMask;
    public Transform spawnBulletPosition;
    private UnityEngine.Object pfBulletProjectile;
    private InputMap input;
    [HideInInspector] public bool IsMouseLeftDown;

    //private bool _isShooting = false;
    private bool isShooting = false;
    private float shootDelay { get { return 1 / RateOfFire; } }
    private float shootWait = 0;
    private bool allowShoot { get { return shootWait >= shootDelay; } }
    private Animator _anim;
    private LayerMask enemyLayer;
    private void Awake()
    {
        Camera = FindObjectOfType<CameraManager>().gameObject.GetComponent<Camera>();
        pfBulletProjectile = Resources.Load("Prefab/Player/pfBulletProjectile");
        IsMouseLeftDown = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        _anim = GetComponent<Animator>();
        input = FindObjectOfType<ControlManager>().InputMap;

        shootWait += shootDelay;
    }

    // Update is called once per frame
    void Update()
    {
        shootWait += Time.deltaTime;
        if (allowShoot && isShooting)
        {
            shootWait = 0;
            Vector3 hitPosition = GetHitPosition();
            Vector3 aimDir = (hitPosition - spawnBulletPosition.position).normalized;
            var bullet = Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up)).GetComponent<BulletProjectile>();
            bullet.collisionLayer = enemyLayer;
            bullet.sourceName = "Player";
        }
    }

    public void HandleShoot(bool performed, bool canceled)
    {
        bool canShoot = !_anim.GetBool("RangeStarting") && _anim.GetBool("IsRangeReady");

        if (performed && !canShoot)
        {
            IsMouseLeftDown = true;

        }
        else if (canceled && !canShoot)
        {
            IsMouseLeftDown = false;
        }
        else if (performed)
        {
            IsMouseLeftDown = true;
            isShooting = true;
        }
        else if (canceled)
        {
            IsMouseLeftDown = false;
            isShooting = false;
        }
    }

    public Vector3 GetHitPosition() {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.ScreenPointToRay(screenCenter);
        Vector3 hitPosition = ray.origin + ray.direction * 500f;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 500f, aimColliderLayerMask))
            hitPosition = raycastHit.point;
        return hitPosition;
    }

}
