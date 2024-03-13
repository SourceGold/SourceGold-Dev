using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public enum Action { Equip, Unequip };


    public Transform []Weapon;
    public Transform []WeaponHandle;
    public Transform []WeaponRestPose;
    public Transform []Shield;
    public Transform []ShieldHandle;
    public Transform []Gun;
    public Transform[] GunHandle;

    public struct WeaponInfo
    {
        public string name;
        public float damge;
    }

    //private PlayerManager _playerManager;
    private Animator _anim;

    private int _weaponType = 0;
    private WeaponInfo[] _weaponInfo;
    private int _shieldType = 0;
    private int _gunType = 0;

    private void Awake()
    {
        //_playerManager = FindObjectOfType<PlayerManager>();
        _anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        
        _anim.SetInteger("WeaponType", 1);
        _weaponInfo = new WeaponInfo[Weapon.Length];
        for (int i = 0; i < Weapon.Length; i++)
        {
            _weaponInfo[i].name = Weapon[i].gameObject.name;

            Weapon[i].SetParent(WeaponRestPose[i]);
            Weapon[i].localRotation = Quaternion.identity;
            Weapon[i].localPosition = Vector3.zero;
            Weapon[i].gameObject.SetActive(false);
        }
        Weapon[0].gameObject.SetActive(true);

        for (int i = 0; i < Shield.Length; i++)
        {
            Shield[i].SetParent(ShieldHandle[i]);
            Shield[i].localRotation = Quaternion.identity;
            Shield[i].localPosition = Vector3.zero;
            Shield[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < Gun.Length; i++)
        {
            Gun[i].SetParent(GunHandle[i]);
            Gun[i].localRotation = Quaternion.identity;
            Gun[i].localPosition = Vector3.zero;
            Gun[i].gameObject.SetActive(false);
        }
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
            SwitchWeapon((_weaponType + 1) % Weapon.Length);
        }
            
    }

    public void SwitchWeapon(int weaponType)
    {
        if (_weaponType == weaponType)
            return;
        Weapon[_weaponType].gameObject.SetActive(false);
        _weaponType = weaponType;
        _anim.SetInteger("WeaponType", _weaponType+1);
        Weapon[_weaponType].gameObject.SetActive(true);
    }

    public void ToggleShield()
    {
        Shield[_shieldType].gameObject.SetActive(!Shield[_shieldType].gameObject.activeSelf);
    }

    public void ToggleGun()
    {
        Gun[_gunType].gameObject.SetActive(!Gun[_gunType].gameObject.activeSelf);
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
