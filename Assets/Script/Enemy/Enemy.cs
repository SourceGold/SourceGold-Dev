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

    private EnemyHealthBar _enemyHealthBar;
    private string _name { get; set; }

    protected void Awake()
    {
        _name = this.transform.name;
    }

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        WeaponHandlerRef = GetComponentInParent<AllEnemyManager>().Player.GetComponentInChildren<WeaponHandler>();
        _enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        RegisterSelf();
    }

    private void RegisterSelf()
    {
        var enemy = new Assets.Script.Backend.Enemy(_name);
        Backend.GameLoop.RegisterGameObject(enemy);
        enemy.SetOnStatsChangedCallback(_enemyHealthBar.EnemyStatsChangeCallback, true);
        EventManager.StartListening(GameEventTypes.GetObjectOnDeathEvent(_name), DeathHandler);
    }

    // Update is called once per frame
    void Update()
    {

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
                var attackerName = other.GetComponentInParent<PlayerManager>().name;
                //_anim.SetTrigger("Hit");
                //Debug.Log("Hit By Sword");

                Backend.GameLoop.ProcessDamage(new DamangeSource(){ SrcObjectName = attackerName }, new DamageTarget() { TgtObjectName = _name });
            } 
        }
    }


}
