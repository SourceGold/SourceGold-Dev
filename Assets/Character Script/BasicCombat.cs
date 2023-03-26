using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    public Animator _anim;
    private int mode;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (mode)
        {
            case 1:
                _anim.Play("MeleeAttack_OneHanded");
                break;
            case 2:
                _anim.Play("MeleeAttack_TwoHanded");
                break;
            case 3:
                _anim.Play("PunchLeft");
                break;
            case 4:
                _anim.Play("PunchRight");
                break;
            case 5:
                _anim.Play("Buff");
                break;
            default:
                //anim.Play("Idle");
                break;

        }
        mode = 0;
    }

    public void MeeleOneHanded()
    {
        mode = 1;
        //_anim.SetBool("combat", true);

    }

    public void MeeletwoHanded()
    {
        mode = 2;
        //_anim.SetBool("combat", true);
    }

    public void PunchLeft()
    {
        mode = 3;
        //_anim.SetBool("combat", true);
    }

    public void PunchRight()
    {
        mode = 4;
        //_anim.SetBool("combat", true);
    }

    public void Buff()
    {
        mode = 5;
        //_anim.SetBool("combat", true);
    }
}
