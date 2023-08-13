#nullable enable

namespace Assets.Script.Backend
{
    public abstract class GameObject
    {
        public string Name { get; set; }

        public GameObjectType Type { get; set; }

        public GameObjectEnvironmentalStats EnvironmentalStats { get; set; }

        public bool RegisteredByGame { get; set; }

        public bool SaveToNextStage { get; set; }

        public GameObject(string name, GameObjectType type, GameObjectEnvironmentalStats environmentalStats, bool saveToNextStage)
        {
            Name = name;
            Type = type;
            EnvironmentalStats = environmentalStats;
            SaveToNextStage = saveToNextStage;
        }

        public abstract void SetGameObjectStates(GameObjectStats GameObjectStats);

        public abstract GameObjectStats GetGameObjectStates();
    }

    public class GameObjectStats
    {
        protected string ParentName { get; private set; }

        public GameObjectStats(string parentName)
        {
            ParentName = parentName;
        }
    }
}
