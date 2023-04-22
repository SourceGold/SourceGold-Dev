using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public WeaponHandler WeaponHandlerRef;
    private Animator _anim;
    //private bool _hit = false;

    // Start is called before the first frame update
    void Start()
    {
        _anim = WeaponHandlerRef.GetComponent<Animator>();
    }
     
    // Update is called once per frame
    void Update()
    {
         
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (_anim.GetBool("IsDamageOn"))
        {
            WeaponHandler.WeaponInfo weaponInfo = WeaponHandlerRef.GetWeaponInfo();
            if (weaponInfo.name == this.gameObject.name)
            {
                Debug.Log("HIT");
                //_hit = true;
            }
        }
    }
}
