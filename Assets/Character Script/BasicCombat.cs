using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    public Animator anim;
    private int mode;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(mode) {
            case 1:
                anim.Play("MeleeAttack_OneHanded");
                break;
            case 2:
                anim.Play("MeleeAttack_TwoHanded");
                break;
            case 3:
                anim.Play("PunchLeft");
                break;
            case 4:
                anim.Play("PunchRight");
                break;
            case 5:
                anim.Play("Buff");
                break;
            default:
                break;

        }
        mode = 0;
    }

    public void MeeleOneHanded()
    {
        //Debug.Log("Attack!!");
        //anim.SetTrigger("hit");
        mode = 1;

    }

    public void MeeletwoHanded()
    {
        mode = 2;

    }

    public void PunchLeft()
    {
        mode = 4;

    }

    public void PunchRight()
    {
        mode = 3;

    }

    public void Buff()
    {
        mode = 5;

    }
}
