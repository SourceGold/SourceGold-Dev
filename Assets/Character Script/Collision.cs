using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    public WeaponHandler WeaponHandlerRef;
    private Animator _anim;
    //private bool _hit = false;
    // Start is called before the first frame update
    void Start()
    {
        _anim = this.GetComponentInParent<Animator>();
    }
     
    // Update is called once per frame
    void Update()
    {
         
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (_anim.GetBool("IsDamageOn") && other.gameObject.tag == "Enemy")
        {
            int weaponType = WeaponHandlerRef.GetWeaponType();
            if (GetWeaponName(weaponType) == this.gameObject.name)
            {
                Debug.Log("HIT");
                //_hit = true;
            }
        }
    }

    private string GetWeaponName(int weaponType)
    {
        string name;
        switch (weaponType)
        {
            case 0:
                name = "Greatsword";
                break;
            case 1:
                name = "Godsword";
                break;
            default:
                name = "Greatsword";
                break;
        }

        return name;
    }
}
