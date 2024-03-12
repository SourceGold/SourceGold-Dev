using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeHandler : MonoBehaviour
{


    //public bool DebugTrail = false;
    //public bool DebugTrail = false;
    //public LayerMask hitLayers;

    //[SerializeField] GameObject hitVFX;

    //public struct BufferObj
    //{
    //    public Vector3 position;
    //    public Quaternion rotation;
    //    public Vector3 size;
    //}
    //public struct BufferObj
    //{
    //    public Vector3 position;
    //    public Quaternion rotation;
    //    public Vector3 size;
    //}

    //private LinkedList<BufferObj> _trailList = new LinkedList<BufferObj>();
    //private BoxCollider _weaponCollider;
    //private int _maxFrameBuffer = 10;

    private Animator _anim;
    private MovementHandler _movementHandler;
    private WeaponHandler _weaponHandler;

    private void Awake()
    {

    }

    void Start()
    {
        _anim = GetComponent<Animator>();
        _movementHandler = GetComponent<MovementHandler>();
        _weaponHandler = GetComponent<WeaponHandler>();
    }

    void Update()
    {

    }

    public void EquipWeapon()
    {
        if (!_anim.GetBool("IsAttacking") && !_anim.GetBool("IsEquipting"))
        {
            if (_anim.GetBool("IsBattlePoseSwitched"))
            {
                SwitchBattlePose();
                _movementHandler.SwitchBattlePose();
            }         
            _anim.SetBool("IsWeaponEquipped", !_anim.GetBool("IsWeaponEquipped"));
            _movementHandler.SetWeaponStatus(_anim.GetBool("IsWeaponEquipped"));
        }
    }

    public void SwitchWeapon()
    {
        if (!_anim.GetBool("IsAttacking") && !_anim.GetBool("IsEquipting") && _anim.GetBool("IsWeaponEquipped"))
        {
            if (_anim.GetBool("IsBattlePoseSwitched"))
            {
                SwitchBattlePose();
                _movementHandler.SwitchBattlePose();
            }
            _anim.SetBool("IsWeaponEquipped", false);
            _anim.SetBool("Switch", true);
            _anim.SetBool("IsSwitching", true);
        }
    }

    public void StandingMeleeLight()
    {
            
        if (_anim.GetBool("CanAttack") && !_anim.GetBool("IsRolling"))
        {
            _anim.SetInteger("AttackType", 1);
            _anim.SetTrigger("Attack");
        }
        if (_anim.GetBool("IsAttacking"))
        {
            _anim.SetBool("IsCombo", true);
        }
    }

    public void StandingMeleeHeavy()
    {
        if (_anim.GetBool("CanAttack") && !_anim.GetBool("IsRolling"))
        {
            _anim.SetInteger("AttackType", 2);
            _anim.SetTrigger("Attack");    
        }
    }

    public void SwitchBattlePose()
    {
        if (_anim.GetBool("CanAttack") && !_anim.GetBool("IsAttacking"))
        {
            _anim.SetBool("IsBattlePoseSwitched", !_anim.GetBool("IsBattlePoseSwitched"));
            switch (_anim.GetInteger("WeaponType"))
            {
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        } 
    }

    public void AttackRelease()
    {
        _anim.SetBool("AttackRelease", true);
    }

    //public void StandingMeleeAttack2Press(bool performed)
    //{
    //    if (performed && _anim.GetBool("CanAttack"))
    //    {
    //        _anim.SetTrigger("Attack");
    //        _anim.SetInteger("AttackType", 2);
    //        _anim.SetBool("AttackRelease", false);
    //    }
    //}

    //public void StandingMeleeAttack2Release(bool performed)
    //{
    //    if (performed)
    //        _anim.SetBool("AttackRelease", true);
    //}



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
    //        }
    //    }
    //}
    //public void HitVFX(BufferObj bo)
    //{
    //    GameObject hit = Instantiate(hitVFX, bo.position, bo.rotation);
    //    Destroy(hit, 0.7f);
    //}
}
