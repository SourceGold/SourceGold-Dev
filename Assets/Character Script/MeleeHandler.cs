using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeHandler : MonoBehaviour
{


    public bool DebugTrail = false;
    public LayerMask hitLayers;

    [SerializeField] GameObject hitVFX;

    public struct BufferObj
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 size;
    }

    private LinkedList<BufferObj> _trailList = new LinkedList<BufferObj>();

    public WeaponHandler WeaponHandlerRef;
    private BoxCollider _weaponCollider;
    private int _maxFrameBuffer = 10;
    Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_anim.GetBool("IsDamageOn"))
        {
            //Debug.Log("Debug");
            CheckTrail();
            //OnDramGizmos();
        }
        
    }

    private void CheckTrail()
    {
        BufferObj bo = new BufferObj();
        bo.size = _weaponCollider.size;
        bo.rotation = _weaponCollider.transform.rotation;
        bo.position = _weaponCollider.transform.position + _weaponCollider.transform.TransformDirection(_weaponCollider.center);
        _trailList.AddFirst(bo);
        if (_trailList.Count > _maxFrameBuffer)
        {
            _trailList.RemoveLast();
        }
        //Debug.Log("Trail");

        Collider[] hits = Physics.OverlapBox(bo.position, bo.size / 2, bo.rotation,
            hitLayers, QueryTriggerInteraction.Ignore);

        //Debug.Log(hits.Length);

        //Dictionary<long, Collider> colliderList = new Dictionary<long, Collider>();
        //for (int i = 0; i < hits.Length; i++)
        //{
        //    colliderList.Add(hits[i].GetInstanceID(), hits[i]);
        //}

        if (hits.Length > 0)
        {
            //Debug.Log(hits[0].tag);
            HitVFX(bo);
        }


    }

    private void OnDrawGizmos()
    {
        //Debug.Log("Draw");
        if (DebugTrail)
        {
            foreach (BufferObj bo in _trailList)
            {
                Gizmos.color = Color.black;
                Gizmos.matrix = Matrix4x4.TRS(bo.position, bo.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, bo.size);

            }
        }
    }

    public void EquipWeapon(InputAction.CallbackContext context)
    {
        if (context.performed && !_anim.GetBool("IsAttacking") && !_anim.GetBool("IsEquipting"))
        {
            _anim.SetBool("IsWeaponEquipped", !_anim.GetBool("IsWeaponEquipped"));
        }
    }

    public void StandingMeleeAttack1(InputAction.CallbackContext context)
    {
        if (context.performed && _anim.GetBool("CanAttack"))
        {
            _anim.SetTrigger("Attack");
            _anim.SetInteger("AttackType", 1);
        }
    }

    public void StandingMeleeAttack2(InputAction.CallbackContext context)
    {
        if (context.performed && _anim.GetBool("CanAttack"))
        {
            _anim.SetTrigger("Attack");
            _anim.SetInteger("AttackType", 2);
        }
    }

    public void RunJumpMeleeAttack(InputAction.CallbackContext context)
    {
        if (context.performed && _anim.GetBool("CanAttack"))
        {
            _anim.SetTrigger("Attack");
            _anim.SetInteger("AttackType", 3);
        }
    }

    public void HitVFX(BufferObj bo)
    {
        GameObject hit = Instantiate(hitVFX, bo.position, bo.rotation);
        Destroy(hit, 0.4f);
    }

    public void setCollider(BoxCollider collider)
    {
        _weaponCollider = collider;
    }
}
