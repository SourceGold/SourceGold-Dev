using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public enum Action { Equip, Unequip };

    
    public Transform []Weapon;
    public Transform []WeaponHandle;
    public Transform []WeaponRestPose;
    public MeleeHandler MeleeHandlerRef;

    private int _weaponType = 0;

    void Start()
    {
        MeleeHandlerRef.setCollider(Weapon[0].GetComponent<BoxCollider>());
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
            _weaponType = (_weaponType + 1) % 2;
            MeleeHandlerRef.setCollider(Weapon[_weaponType].GetComponent<BoxCollider>());
        }
            
    }

    public Collider GetCollider()
    {
        return Weapon[_weaponType].GetComponent<Collider>();
    }

    public int GetWeaponType()
    {
        return _weaponType;
    }
}
