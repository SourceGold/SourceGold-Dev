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
    [HideInInspector] public bool IsMouseLeftDown;

    private bool isShooting = false;
    private float ShootDelay => 1 / RateOfFire;
    private float shootWait = 0;
    private bool AllowShoot => shootWait >= ShootDelay;
    private Animator _anim;
    private LayerMask enemyLayer;

    private CameraManager _cameraManager;
    private MovementHandler _movementHandler;
    private WeaponHandler _weaponHandler;

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
        _cameraManager = FindObjectOfType<CameraManager>();
        _movementHandler = GetComponent<MovementHandler>();
        _weaponHandler = GetComponent<WeaponHandler>();

        shootWait += ShootDelay;
    }

    // Update is called once per frame
    void Update()
    {
        shootWait += Time.deltaTime;
        if (AllowShoot && isShooting)
        {
            shootWait = 0;
            Vector3 hitPosition = GetHitPosition();
            Vector3 aimDir = (hitPosition - spawnBulletPosition.position).normalized;
            var bullet = Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up)).GetComponent<BulletProjectile>();
            bullet.CollisionLayer = enemyLayer;
            bullet.SourceName = transform.name;
        }
    }

    public void ToggleAim()
    {
        if (!_anim.GetBool("IsRolling") && !_anim.GetBool("RangeStarting") && !_anim.GetBool("IsWeaponReady") && !_anim.GetBool("IsEquipting") && !IsMouseLeftDown)
        {
            _cameraManager.ToggleAim();
            _weaponHandler.ToggleGun();
            _anim.SetBool("IsRangeStart", !_anim.GetBool("IsRangeStart"));
            _movementHandler.ToggleAim(_anim.GetBool("IsRangeStart"));
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

    public Vector3 GetHitPosition() 
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.ScreenPointToRay(screenCenter);
        Vector3 hitPosition = ray.origin + ray.direction * 500f;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 500f, aimColliderLayerMask))
            hitPosition = raycastHit.point;
        return hitPosition;
    }

}
