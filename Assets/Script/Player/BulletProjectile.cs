using Assets.Script;
using Assets.Script.Backend;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public Transform vfxHitRed;
    public float BulletSpeed = 200f;
    public float VanishTime = 4f;
    public LayerMask CollisionLayer;
    public string SourceName;
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

    private void Hit(RaycastHit hit)
    {
        Instantiate(vfxHitRed, hit.point, Quaternion.identity);
        if (CollisionLayer == 1 << hit.transform.gameObject.layer)
        {
            Backend.GameLoop.ProcessDamage(new DamageSource() { SrcObjectName = SourceName, AttackWeapon = WeaponNames.Ranged1 },
                new DamageTarget() { TgtObjectName = hit.transform.name == "Player Bot" ? "Player" : hit.transform.name });
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Player Bot")
        {
            if (!other.transform.GetComponent<Animator>().GetBool("IsInvincible"))
                Backend.GameLoop.ProcessDamage(new DamageSource() { SrcObjectName = "Player", AttackWeapon = WeaponNames.Ranged1 },
                    new DamageTarget() { TgtObjectName = "Player" });
        }
    }

    private void FixedUpdate()
    {
        float maxDistance = BulletSpeed * Time.fixedDeltaTime;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance))
        {
            if (hit.transform.name == "Player Bot")
            {
                if (hit.transform.GetComponent<Animator>().GetBool("IsInvincible"))
                {
                    if (Physics.Raycast(hit.point, transform.forward, out RaycastHit nextHit,
                        maxDistance - Vector3.Distance(hit.point, transform.position), ~LayerMask.GetMask("Player")))
                    {
                        Hit(nextHit);
                    }
                }
                else
                {
                    Hit(hit);
                }
            }
            else
            {
                Hit(hit);
            }
        }
    }

}
