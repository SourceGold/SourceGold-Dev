namespace Assets.Script.Backend
{
    public class GameEventTypes
    {
        public static GameEventType FetchNewGameObjectsEvent => new GameEventType(nameof(FetchNewGameObjectsEvent));

        public static GameEventType GetObjectOnDeathEvent(string enemyName) => new GameEventType($"{enemyName}DeathEvent");

        public static GameEventType GetInventoryFullEvent => new GameEventType(nameof(GetInventoryFullEvent));

        public static GameEventType GetItemReachedMaxCountEvent => new GameEventType(nameof(GetItemReachedMaxCountEvent));

        public static GameEventType GetNotEnoughStatsEvent(string stats) => new GameEventType($"NotEnough{stats}Event");
    }

    public class GameEventType
    {
        private string _name { get; set; }

        internal GameEventType(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }

}
