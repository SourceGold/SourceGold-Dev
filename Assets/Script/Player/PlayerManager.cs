using Assets.Script;
using Assets.Script.Backend;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager, IDataPersistence
{
    [HideInInspector] public Transform MainCamera;
    private WeaponHandler _weaponHandler;
    private GameObject _player;
    private Transform _playerBot;
    private MovementHandler _movementHandler;

    private void Awake()
    {
        MainCamera = FindObjectOfType<CameraManager>().GetComponent<Transform>();
        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        _movementHandler = GetComponentInChildren<MovementHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Backend.GameLoop.RegisterGameObject(new PlayableCharacter(this.name));
        ((IDataPersistence)this).RegisterExistence();
        //Debug.Log();
        _player = Resources.Load("Prefab/Player Bot") as GameObject;
        _playerBot = transform.Find("Player Bot");
        //Debug.Log(GameEventLogger.Instance.PlayerStats);
        //if (GameEventLogger.Instance.PlayerStats != null) {
        //    _playerBot.transform.position = new Vector3(GameEventLogger.Instance.PlayerStats.X, GameEventLogger.Instance.PlayerStats.Y, GameEventLogger.Instance.PlayerStats.Z);
        //    Debug.Log(GameEventLogger.Instance.PlayerStats.X);
        //    var fileName = $"{GetSaveFileName()}.yaml";
        //    var fullSaveFileName = Path.Combine("save1", fileName);
        //    var fullPath = Path.Combine(Application.persistentDataPath, fullSaveFileName);
        //    Debug.Log(DataPersistenceManager.LoadDataFile<PlayerSaveInfo>(fullPath).X);
        //}
        //Debug.Log(MainCamera);
        first = true;
    }

    private void Reset()
    {
        //if (GameEventLogger.Instance.PlayerStats != null)
        //{
        //    _playerBot.transform.position = new Vector3(GameEventLogger.Instance.PlayerStats.X, GameEventLogger.Instance.PlayerStats.Y, GameEventLogger.Instance.PlayerStats.Z);
        //    Debug.Log(GameEventLogger.Instance.PlayerStats.X);
        //    var fileName = $"{GetSaveFileName()}.yaml";
        //    var fullSaveFileName = Path.Combine("save1", fileName);
        //    var fullPath = Path.Combine(Application.persistentDataPath, fullSaveFileName);
        //    Debug.Log(DataPersistenceManager.LoadDataFile<PlayerSaveInfo>(fullPath).X);   
        //}
    }

    private bool first = true;

    // Update is called once per frame
    void Update()
    {
        // This is a test function to auto seft damage
        //AutoSelfDmg();
        //live();
        if (first)
        {
            first = false;
            if (GameEventLogger.Instance.PlayerStats == null)
            {
                return;
            }
            Debug.Log(GameEventLogger.Instance.PlayerStats.X);
            _movementHandler.Teleport(GameEventLogger.Instance.PlayerStats.V3Position());
            //var pp = _player.transform.position;
            //var newP = pp + new Vector3(-10f, 0f, 17f);
            //_playerBot.transform.localPosition = newP;

        }
    }

    public void live()
    {
        if (!_start)
        {
            Invoke("live2", 10.0f);
            _start = true;
        }

    }

    public void live2()
    {
        Debug.Log("Live");
        Invoke("live2", 10);

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
        // TODO: Remove old register before restarting the player manager 
        var playerInfo = DataPersistenceManager.LoadDataFile<PlayerSaveInfo>(fileName);
        Debug.Log($"loaded info: x {playerInfo.X}, y {playerInfo.Y}, z {playerInfo.Z}");

        //Instantiate(_player, new Vector3(-10, 2, 30), Quaternion.identity);
        //if (transform.Find("Player Bot") != null)
        //{
        //    Destroy(transform.Find("Player Bot").gameObject);
        //}
        GameEventLogger.Instance.PlayerStats = playerInfo;
        Debug.Log(GameEventLogger.Instance.PlayerStats);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //_playerBot.transform.position = new Vector3(playerInfo.X, playerInfo.Y, playerInfo.Z);
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
            WeaponType = _weaponHandler.WeaponType,
            WeaponDrawn = _weaponHandler.WeaponDrawn
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
