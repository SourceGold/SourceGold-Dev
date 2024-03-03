namespace Assets.Script.Backend
{
    public class Backend : Singleton<Backend>
    {
        public static GameSceneBase GameLoop;

        public PlayerSaveInfo PlayerStats = null;

        public new void Awake()
        {
            GameEventLogger.LogEvent("Game Backend Awaken", EventLogType.SystemEvent);
            GameLoop = new GameSceneTest();
            GameLoop.InitializeStage();
            this.doNotDestoryOnLoad = true;
            base.Awake();
        }

        // test only
        public void Start()
        {
            //DataPersistenceManager.LoadGame(DataPersistenceManager._testSave);
        }

        public void SetNextStage(string stageName)
        {
            // broadcast to all game objects the new state
            var newStage = LoadStage(stageName);
            newStage.InitializeStage(GameLoop);
            GameLoop = newStage;
        }

        // test only
        public void OnApplicationQuit()
        {
            DataPersistenceManager.SaveGame(DataPersistenceManager._testSave);
        }

        private GameSceneBase LoadStage(string stageName)
        {
            return new GameSceneTest();
        }
    }
}
