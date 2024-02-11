using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeHandler : MonoBehaviour
{


    //public bool DebugTrail = false;
    //public LayerMask hitLayers;

    //[SerializeField] GameObject hitVFX;

    //public struct BufferObj
    //{
    //    public Vector3 position;
    //    public Quaternion rotation;
    //    public Vector3 size;
    //}

    //private LinkedList<BufferObj> _trailList = new LinkedList<BufferObj>();
    //private BoxCollider _weaponCollider;
    //private int _maxFrameBuffer = 10;

    public WeaponHandler WeaponHandlerRef;
    private Animator _anim;

    private void Awake()
    {

    }

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {

    }

    public void EquipWeapon(bool performed)
    {
        if (performed && !_anim.GetBool("IsAttacking") && !_anim.GetBool("IsEquipting"))
        {
            _anim.SetBool("IsWeaponEquipped", !_anim.GetBool("IsWeaponEquipped"));
        }
    }

    public void SwitchWeapon(bool performed)
    {
        if (performed && !_anim.GetBool("IsAttacking") && !_anim.GetBool("IsEquipting") && _anim.GetBool("IsWeaponEquipped"))
        {
            _anim.SetBool("IsWeaponEquipped", false);
            _anim.SetBool("Switch", true);
            _anim.SetBool("IsSwitching", true);
        }
    }


    public void StandingMeleeAttack1(bool performed)
    {
        if (performed)
        {
            _anim.SetInteger("AttackType", 1);
            if (_anim.GetBool("CanAttack"))
            {
                _anim.SetTrigger("Attack");
            }
            if (_anim.GetBool("IsAttacking"))
            {
                _anim.SetBool("IsCombo", true);
            }
        }
    }

    public void StandingMeleeAttack2Press(bool performed)
    {
        if (performed && _anim.GetBool("CanAttack"))
        {
            _anim.SetTrigger("Attack");
            _anim.SetInteger("AttackType", 2);
            _anim.SetBool("AttackRelease", false);
        }
    }

    public void StandingMeleeAttack2Release(bool performed)
    {
        if (performed)
            _anim.SetBool("AttackRelease", true);
    }

    public void StandingMeleeAttack3(bool performed)
    {
        if (performed && _anim.GetBool("CanAttack"))
        {
            _anim.SetTrigger("Attack");
            _anim.SetInteger("AttackType", 3);
        }
    }


    //private void CheckTrail()
    //{
    //    BufferObj bo = new BufferObj();
    //    bo.size = _weaponCollider.size;
    //    bo.rotation = _weaponCollider.transform.rotation;
    //    bo.position = _weaponCollider.transform.position + _weaponCollider.transform.TransformDirection(_weaponCollider.center);
    //    _trailList.AddFirst(bo);
    //    if (_trailList.Count > _maxFrameBuffer)
    //    {
    //        _trailList.RemoveLast();
    //    }

    //    Collider[] hits = Physics.OverlapBox(bo.position, bo.size / 2, bo.rotation,
    //        hitLayers, QueryTriggerInteraction.Ignore);

    //    if (hits.Length > 0)
    //    {
    //        //Debug.Log(hits[0].tag);
    //        HitVFX(bo);
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    Debug.Log("Draw");
    //    if (DebugTrail)
    //    {
    //        foreach (BufferObj bo in _trailList)
    //        {
    //            Gizmos.color = Color.black;
    //            Gizmos.matrix = Matrix4x4.TRS(bo.position, bo.rotation, Vector3.one);
    //            Gizmos.DrawWireCube(Vector3.zero, bo.size);

    //        }
    //    }
    //}
    //public void HitVFX(BufferObj bo)
    //{
    //    GameObject hit = Instantiate(hitVFX, bo.position, bo.rotation);
    //    Destroy(hit, 0.7f);
    //}
}
