using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public Transform vfxHitGreen;
    public Transform vfxHitRed;

    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 200f;
        bulletRigidbody.velocity = transform.forward * speed;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //Debug.Log(other);
    //    Instantiate(vfxHitRed, transform.position, Quaternion.identity);
    //    Destroy(gameObject);
    //}

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        //Debug.Log(collision);
        Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
