using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.Backend;

public class Enemy : MonoBehaviour
{
    private WeaponHandler WeaponHandlerRef;
    [SerializeField] private float Health;
    private Animator _anim;
    private bool _dead = false;

    private string _name = "EnemyDefault";
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        WeaponHandlerRef = GetComponentInParent<EnemyManager>().Player.GetComponentInChildren<WeaponHandler>();

        EventManager.StartListening(GameEventTypes.GetObjectOnDeathEvent(_name), DeathHandler);
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

    private void DeathHandler()
    {
        this.gameObject.SetActive(false);
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
                
                Backend.GameLoop.ProcessDamage(new DamangeSource(){SrcObjectName= "PlayerDefault" }, new DamageTarget() { TgtObjectName = _name });
            } 
        }
    }


}
