using Assets.Script;
using Assets.Script.Backend;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager, IDataPersistence
{
    [HideInInspector] public Transform MainCamera;
    public WeaponHandler _weaponHandler;
    private MovementHandler _movementHandler;
    private Animator _anim;

    private void Awake()
    {
        MainCamera = FindObjectOfType<CameraManager>().GetComponent<Transform>();
        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        _movementHandler = GetComponentInChildren<MovementHandler>();
        _anim = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Backend.GameLoop.RegisterGameObject(new PlayableCharacter(this.name));
        ((IDataPersistence)this).RegisterExistence();

        var playerStats = Backend.Instance.PlayerStats;
        if (playerStats == null)
        {
            return;
        }

        _movementHandler.Teleport(playerStats.V3Position());

        if (playerStats.WeaponDrawn)
        {
            _anim.SetBool("IsWeaponEquipped", true);
        }
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

    public void LoadData(string fileName)
    {
        var playerInfo = DataPersistenceManager.LoadDataFile<PlayerSaveInfo>(fileName);
        Debug.Log($"loaded info: x {playerInfo.X}, y {playerInfo.Y}, z {playerInfo.Z}");

        Backend.Instance.PlayerStats = playerInfo;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnDestroy()
    {
        ((IDataPersistence)this).Dispose();
    }

    public void SaveData(string fileName)
    {
        if (this.IsUnityNull())
        {
            return;
        }
        var playerTranform = transform.Find("Player Bot").transform.localPosition;
        var playerInfo = new PlayerSaveInfo()
        {
            X = playerTranform.x,
            Y = playerTranform.y,
            Z = playerTranform.z,
        };

        Debug.Log($"saved info: x {playerInfo.X}, y {playerInfo.Y}, z {playerInfo.Z}");
        DataPersistenceManager.SaveDataFile(fileName, playerInfo);
    }

    public string GetSaveFileName()
    {
        return nameof(PlayerSaveInfo);
    }
}

public class PlayerSaveInfo
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public int WeaponType { get; set; }
    public bool WeaponDrawn { get; set; }

    public Vector3 V3Position()
    {
        return new Vector3(X, Y, Z);
    }
}
