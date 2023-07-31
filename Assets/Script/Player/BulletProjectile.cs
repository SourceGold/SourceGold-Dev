using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BulletProjectile : MonoBehaviour
{
    public Transform vfxHitGreen;
    public Transform vfxHitRed;
    public float BulletSpeed = 200f;
    public float VanishTime = 4f;
    public LayerMask Border;

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

    private void OnTriggerEnter(Collider other)
    {
        if (Border != (Border | (1 << other.transform.gameObject.layer))) // check layer
        {
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, BulletSpeed * Time.fixedDeltaTime))
        {
            if (Border != (Border | (1 << hit.transform.gameObject.layer))) // check layer
            {
                Instantiate(vfxHitRed, hit.point, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

}
