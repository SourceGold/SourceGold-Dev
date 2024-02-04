using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingHandler : MonoBehaviour
{
    public float RateOfFire = 50f;
    private Camera Camera;
    public LayerMask aimColliderLayerMask;
    public Transform spawnBulletPosition;
    public Transform pfBulletProjectile;
    private InputMap input;

    [HideInInspector] public bool IsMouseLeftDown;

    //private bool _isShooting = false;
    private bool isShooting = false;
    private float shootDelay { get { return 1 / RateOfFire; } }
    private float shootWait = 0;
    private bool allowShoot { get { return shootWait >= shootDelay; } }
    private Animator _anim;

    private void Awake()
    {
        Camera = FindObjectOfType<CameraManager>().gameObject.GetComponent<Camera>();
        //spawnBulletPosition = transform.Find("BulletPosition");
        IsMouseLeftDown = false;
    }

    public void handleShoot(InputAction.CallbackContext context)
    {
        bool canShoot = !_anim.GetBool("RangeStarting") && _anim.GetBool("IsRangeReady");

        if (context.performed && !canShoot)
        {
            IsMouseLeftDown = true;

        }
        else if (context.canceled && !canShoot)
        {
            IsMouseLeftDown = false;    
        }
        else if (context.performed)
        {
            IsMouseLeftDown = true;
            isShooting = true;
        }
        else if (context.canceled)
        {
            IsMouseLeftDown = false;
            isShooting = false;
        }

        //if (context.started)
        //    if (!isShooting)
        //        isShooting = true;

        //if (context.canceled)
        //    isShooting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        input = FindObjectOfType<ControlManager>().InputMap;
        input.Player.Shoot.started += handleShoot;
        input.Player.Shoot.performed += handleShoot;
        input.Player.Shoot.canceled += handleShoot;
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
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
     
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
