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
    public LayerMask collisionLayer;
    public string sourceName;
    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bulletRigidbody.velocity = transform.forward * BulletSpeed;
        Destroy(gameObject, VanishTime);
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, BulletSpeed * Time.fixedDeltaTime))
        {
            Instantiate(vfxHitRed, hit.point, Quaternion.identity);
            if (collisionLayer == 1 << hit.transform.gameObject.layer)
            {
                Backend.GameLoop.ProcessDamage(new DamangeSource() { SrcObjectName = sourceName },
                    new DamageTarget() { TgtObjectName = hit.transform.parent.name });
            }
            Destroy(gameObject);
        }
    }

}
