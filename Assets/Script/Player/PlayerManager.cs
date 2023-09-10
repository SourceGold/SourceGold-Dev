using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    private string _playerName = "PlayerDefault";
    
    [HideInInspector] public Transform MainCamera;

    private void Awake()
    {
        MainCamera = FindObjectOfType<CameraManager>().GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Backend.GameLoop.RegisterGameObject(new PlayableCharacter(_playerName));
        
        //Debug.Log(MainCamera);
    }

    private bool a = false;
    // Update is called once per frame
    void Update()
    {
        if (!a)
        {
            a = true;
            Invoke("kouxue", 5.0f);
        }
    }


    public void kouxue()
    {
        Backend.GameLoop.ProcessDamage(new DamangeSource() { SrcObjectName = "EnemyDefault" }, new DamageTarget() { TgtObjectName = _playerName });
        Invoke("kouxue", 5.0f);
    }
}
