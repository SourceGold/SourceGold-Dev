using Assets.Script.Backend;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public Transform MainCamera;

    private void Awake()
    {
        MainCamera = FindObjectOfType<CameraManager>().GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Backend.GameLoop.RegisterGameObject(new PlayableCharacter(this.name));
        //Debug.Log(MainCamera);
    }

    // Update is called once per frame
    void Update()
    {
        // This is a test function to auto seft damage
        //AutoSelfDmg();
    }

    private bool _start = false;
    public void AutoSelfDmg()
    {
        if (!_start)
        {
            Invoke("SelfDmg", 10.0f);
            _start = true;
        }
    }

    public void SelfDmg()
    {
        Backend.GameLoop.ProcessDamage(new DamangeSource() { SrcObjectName = this.name }, new DamageTarget() { TgtObjectName = this.name });
        Invoke("SelfDmg", 10.0f);
    }
}
