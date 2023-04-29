using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public enum Action { Equip, Unequip };


    public string[] Name;
    public float[] Damage;
    public Transform []Weapon;
    public Transform []WeaponHandle;
    public Transform []WeaponRestPose;
    public MeleeHandler MeleeHandlerRef;

    public struct WeaponInfo
    {
        public string name;
        public float damge;
    }

    private Animator _anim;
    private int _weaponType = 0;
    private WeaponInfo[] _weaponInfo;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.SetInteger("WeaponType", 1);
        _weaponInfo = new WeaponInfo[Weapon.Length];
        for (int i = 0; i < Weapon.Length; i++)
        {
            _weaponInfo[i].name = Name[i];
            _weaponInfo[i].damge = Damage[i];

            Weapon[i].SetParent(WeaponRestPose[i]);
            Weapon[i].localRotation = Quaternion.identity;
            Weapon[i].localPosition = Vector3.zero;
        }
        Weapon[1].gameObject.SetActive(false);
    }

    public void ResetWeapon(Action action, bool switchWeapon)
    {
        if (action == Action.Equip)
        {
            Weapon[_weaponType].SetParent(WeaponHandle[_weaponType]);
           
        }
        else 
        {
            Weapon[_weaponType].SetParent(WeaponRestPose[_weaponType]);
            
        }

        Weapon[_weaponType].localRotation = Quaternion.identity;
        Weapon[_weaponType].localPosition = Vector3.zero;

        if (switchWeapon)
        {
            Weapon[_weaponType].gameObject.SetActive(false);
            _weaponType = (_weaponType + 1) % Weapon.Length;
            _anim.SetInteger("WeaponType", _weaponType+1);
            Weapon[_weaponType].gameObject.SetActive(true);
        }
            
    }

    public Collider GetCollider()
    {
        return Weapon[_weaponType].GetComponent<Collider>();
    }

    public WeaponInfo GetWeaponInfo()
    {
        return _weaponInfo[_weaponType];
    }
}
