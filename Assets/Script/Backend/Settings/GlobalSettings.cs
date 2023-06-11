namespace Assets.Script.Backend
{
    public class GlobalSettings
    {
        public GamePlay GamePlay { get; set; }

        public Camera Camera { get; set; }
    }

    public class Camera
    {
        public int Setting1 { get; set; }
        public int Setting2 { get; set; }
    }

    public class GamePlay
    {
        public int Setting1 { get; set; }
        public int Setting2 { get; set; }

        public bool Setting3 { get; set; }

        public string Setting4 { get; set; }
    }
}
