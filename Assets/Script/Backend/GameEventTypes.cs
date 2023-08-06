namespace Assets.Script.Backend
{
    public class GameEventTypes
    {
        public static GameEventType FetchNewGameObjectsEvent => new GameEventType(nameof(FetchNewGameObjectsEvent));

        public static GameEventType GetObjectOnDeathEvent(string enemyName) => new GameEventType($"{enemyName}DeathEvent");
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
