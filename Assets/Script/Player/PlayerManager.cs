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
        //AutoKouxue();
    }

    private bool _start = false;
    public void AutoKouxue()
    {
        if (!_start)
        {
            Invoke("kouxue", 10.0f);
            _start = true;
        }
    }

    public void kouxue()
    {
        Backend.GameLoop.ProcessDamage(new DamangeSource() { SrcObjectName = this.name }, new DamageTarget() { TgtObjectName = this.name });
        Invoke("kouxue", 10.0f);
    }
}
