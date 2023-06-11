namespace Assets.Script.Backend
{
    public class GameObject
    {
        public string Name { get; set; }

        public GameObjectType Type { get; set; }

        public GameObject(string name, GameObjectType type)
        {
            Name = name;
            Type = type;
        }
    }
}
