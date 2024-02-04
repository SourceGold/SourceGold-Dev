using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public enum Action { Equip, Unequip };


    public string[] Name;
    public float[] Damage;
    private Transform[] Weapon = new Transform[2];
    public Transform []WeaponHandle = new Transform[2];
    private Transform []WeaponRestPose = new Transform[2];
    public MeleeHandler MeleeHandlerRef;

    public struct WeaponInfo
    {
        public string name;
        public float damge;
    }

    private Animator _anim;
    public int WeaponType { get; private set; } = 0;
    private WeaponInfo[] _weaponInfo;
    public bool WeaponDrawn { get; private set; } = false;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.SetInteger("WeaponType", 1);

        Weapon[0] = Instantiate((Resources.Load("Prefab/Greatsword") as GameObject), new Vector3(0, 0, 0), Quaternion.identity).transform;
        Weapon[1] = Instantiate((Resources.Load("Prefab/Godsword") as GameObject), new Vector3(0, 0, 0), Quaternion.identity).transform;
        WeaponRestPose[0] = FindChildRecursive(transform.gameObject, "GreatswordRestPose").transform;
        WeaponRestPose[1] = FindChildRecursive(transform.gameObject, "GodswordRestPose").transform;
        Debug.Log(Weapon[0]);
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
            Weapon[WeaponType].SetParent(WeaponHandle[WeaponType]);
            WeaponDrawn = true;
        }
        else 
        {
            Weapon[WeaponType].SetParent(WeaponRestPose[WeaponType]);
            WeaponDrawn = false;
        }

        Weapon[WeaponType].localRotation = Quaternion.identity;
        Weapon[WeaponType].localPosition = Vector3.zero;

        if (switchWeapon)
        {
            SetWeapon((WeaponType + 1) % Weapon.Length);
        }
            
    }

    public Collider GetCollider()
    {
        return Weapon[WeaponType].GetComponent<Collider>();
    }

    public WeaponInfo GetWeaponInfo()
    {
        return _weaponInfo[WeaponType];
    }

    public GameObject FindChildRecursive(GameObject parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent.transform)
        {
            GameObject result = FindChildRecursive(child.gameObject, name);
            if (result != null) return result;
        }
        return null;
    }

    public void SetWeapon(int weaponType)
    {
        if (weaponType != this.WeaponType)
        {
            Weapon[WeaponType].gameObject.SetActive(false);
            WeaponType = weaponType;
            _anim.SetInteger("WeaponType", WeaponType + 1);
            Weapon[WeaponType].gameObject.SetActive(true);
        }
    }
}
