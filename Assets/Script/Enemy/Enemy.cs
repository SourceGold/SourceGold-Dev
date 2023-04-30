using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public WeaponHandler WeaponHandlerRef;
    [SerializeField] private float Health;
    private Animator _anim;
    private bool _dead = false;
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_dead)
        {
            if (Health <= 0)
            {
                _dead = true;
                _anim.SetBool("Dead", true);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Animator anim_other = other.GetComponentInParent<Animator>();
        if (anim_other.GetBool("IsDamageOn"))
        {
            WeaponHandler.WeaponInfo weaponInfo = WeaponHandlerRef.GetWeaponInfo();
            if (weaponInfo.name == other.gameObject.name)
            {
                //if (_anim.GetCurrentAnimatorStateInfo(1).IsName("Idle"))
                _anim.SetTrigger("Hit");
                Health -= weaponInfo.damge;
                Debug.Log("Hit By Sword");

            } 
        }
    }
}
