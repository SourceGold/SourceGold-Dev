using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Script.Backend;

public class BulletProjectile : MonoBehaviour
{
    public Transform vfxHitRed;
    public float BulletSpeed = 200f;
    public float VanishTime = 4f;
    private LayerMask enemyLayer;

    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bulletRigidbody.velocity = transform.forward * BulletSpeed;
        Destroy(gameObject, VanishTime);
        enemyLayer = LayerMask.GetMask(new string[]{"Enemy"});
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, BulletSpeed * Time.fixedDeltaTime))
        {
            if (hit.transform.gameObject.tag != "Border") // check border
            {
                Instantiate(vfxHitRed, hit.point, Quaternion.identity);
                Destroy(gameObject);
                if (enemyLayer == 1 << hit.transform.gameObject.layer)
                {
                    Backend.GameLoop.ProcessDamage(new DamangeSource() { SrcObjectName = "Player" },
                        new DamageTarget() { TgtObjectName = hit.transform.name });
                }
            }
        }
    }

}
