using System.Collections.Concurrent;

namespace Assets.Script.Backend
{
    public partial class GameSceneBase
    {
        protected ConcurrentDictionary<string, GameObject> AllGameObjectCollection { get; set; }

        protected Inventory PlayerInventory { get; set; }

        public GameSceneBase()
        {
            AllGameObjectCollection = new ConcurrentDictionary<string, GameObject>();
            PlayerInventory = new Inventory(100);
        }

        public void InitializeStage(GameSceneBase previousStage)
        {
            var savedGameObjects = previousStage.GetSavedGameObjects();
            var playerInventory = previousStage.GetInventory();
            InitializeStageGameObject(savedGameObjects);
            InitializeStageInventory(playerInventory);
        }

        public void InitializeStage()
        {
            InitializeCharacters();
        }
    }
}
