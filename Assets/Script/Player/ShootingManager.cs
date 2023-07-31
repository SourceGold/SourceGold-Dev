using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingManager : MonoBehaviour
{

    public float RateOfFire = 4f;
    public Camera Camera;
    public InputAction InputAction;
    public LayerMask aimColliderLayerMask;
    public Transform spawnBulletPosition;
    public Transform pfBulletProjectile;

    //private bool _isShooting = false;
    private bool isShooting = false;
    private float shootDelay { get { return 1 / RateOfFire; } }
    private float shootWait = 0;
    private bool allowShoot { get { return shootWait >= shootDelay; } }

    public void handleShoot(InputAction.CallbackContext context)
    {
        if (context.started)
            if (!isShooting)
                isShooting = true;

        if (context.canceled)
            isShooting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        shootWait += shootDelay;
    }

    // Update is called once per frame
    void Update()
    {
        shootWait += Time.deltaTime;
        if (allowShoot && isShooting)
        {
            shootWait = 0;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.ScreenPointToRay(screenCenter);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
            {
                Vector3 hitPosition = raycastHit.point;
                Vector3 aimDir = (hitPosition - spawnBulletPosition.position).normalized;
                Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            }
        }
    }
}
