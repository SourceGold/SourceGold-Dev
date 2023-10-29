using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeHandler : MonoBehaviour
{


    public bool DebugTrail = false;
    //public LayerMask hitLayers;
    public WeaponHandler WeaponHandlerRef;

    //[SerializeField] GameObject hitVFX;

    public struct BufferObj
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 size;
    }

    private Animator _anim;

    //private LinkedList<BufferObj> _trailList = new LinkedList<BufferObj>();
    //private BoxCollider _weaponCollider;
    //private int _maxFrameBuffer = 10;

    private InputMap input;

    private void Awake()
    {
        input = FindObjectOfType<ControlManager>().InputMap;

        input.Player.MeleeAttack1.performed         += StandingMeleeAttack1;
        input.Player.MeleeAttack2Press.performed    += StandingMeleeAttack2Press;
        input.Player.EquipWeapon.performed          += EquipWeapon;
        input.Player.SkillMeleeAttack1.performed    += SkillMeleeAttack1;
        input.Player.SwitchWeapon.performed         += SwitchWeapon;
        input.Player.MeleeAttack2Release.performed  += StandingMeleeAttack2Release;
        input.Player.MeleeAttack3.performed         += StandingMeleeAttack3;
    }


    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        //if (_anim.GetBool("IsDamageOn"))
        //{
            //CheckTrail();
        //}

    }

    public void EquipWeapon(InputAction.CallbackContext context)
    {
        if (context.performed && !_anim.GetBool("IsAttacking") && !_anim.GetBool("IsEquipting"))
        {
            _anim.SetBool("IsWeaponEquipped", !_anim.GetBool("IsWeaponEquipped"));
        }
    }

    public void SwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.performed && !_anim.GetBool("IsAttacking") && !_anim.GetBool("IsEquipting") && _anim.GetBool("IsWeaponEquipped"))
        {
            _anim.SetBool("IsWeaponEquipped", false);
            _anim.SetBool("Switch", true);
            _anim.SetBool("IsSwitching", true);
        }
    }


    public void StandingMeleeAttack1(InputAction.CallbackContext context)
    {
        if (context.performed)
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

    public void StandingMeleeAttack2Press(InputAction.CallbackContext context)
    {
        if (context.performed && _anim.GetBool("CanAttack"))
        {
            _anim.SetTrigger("Attack");
            _anim.SetInteger("AttackType", 2);
            _anim.SetBool("AttackRelease", false);
        }
    }

    public void StandingMeleeAttack2Release(InputAction.CallbackContext context)
    {
        if (context.performed)
            _anim.SetBool("AttackRelease", true);
    }

    public void StandingMeleeAttack3(InputAction.CallbackContext context)
    {
        if (context.performed && _anim.GetBool("CanAttack"))
        {
            _anim.SetTrigger("Attack");
            _anim.SetInteger("AttackType", 3);
        }
    }

    // Debug
    public void SkillMeleeAttack1(InputAction.CallbackContext context)
    {
        if (context.performed && _anim.GetBool("CanAttack"))
        {
        }
    }


    // Debug
    public void Test(InputAction.CallbackContext context)
    {
        if (context.performed && _anim.GetBool("CanAttack"))
        {
        }
    }

    // Debug
    public void Test1(InputAction.CallbackContext context)
    {
        if (context.performed && _anim.GetBool("IsAttacking"))
        {
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

    private void OnDrawGizmos()
    {
        //Debug.Log("Draw");
        if (DebugTrail)
        {
            //foreach (BufferObj bo in _trailList)
            //{
            //    Gizmos.color = Color.black;
            //    Gizmos.matrix = Matrix4x4.TRS(bo.position, bo.rotation, Vector3.one);
            //    Gizmos.DrawWireCube(Vector3.zero, bo.size);

            //}
        }
    }
    //public void HitVFX(BufferObj bo)
    //{
    //    GameObject hit = Instantiate(hitVFX, bo.position, bo.rotation);
    //    Destroy(hit, 0.7f);
    //}
}
