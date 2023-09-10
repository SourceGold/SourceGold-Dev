#nullable enable

namespace Assets.Script.Backend
{
    public abstract class BackendGameObject
    {
        public string Name { get; set; }

        public GameObjectType Type { get; set; }

        public GameObjectEnvironmentalStats? EnvironmentalStats { get; set; } = null;

        public bool RegisteredByGame { get; set; }

        public bool SaveToNextStage { get; set; }

        public BackendGameObject(string name, GameObjectType type, GameObjectEnvironmentalStats environmentalStats, bool saveToNextStage)
        {
            Name = name;
            Type = type;
            EnvironmentalStats = environmentalStats;
            SaveToNextStage = saveToNextStage;
        }

        public BackendGameObject(string name, GameObjectType type, bool saveToNextStage)
        {
            Name = name;
            Type = type;
            EnvironmentalStats = null;
            SaveToNextStage = saveToNextStage;
        }

        public abstract void SetGameObjectStates(BackendGameObjectStats GameObjectStats);

        public abstract BackendGameObjectStats GetGameObjectStates();
    }

    public abstract class BackendGameObjectStats
    {
        protected string BackendGameObjectName { get; private set; }

        public BackendGameObjectStats(string gameObjectName)
        {
            BackendGameObjectName = gameObjectName;
        }
    }
}
