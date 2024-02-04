using System.Collections.Concurrent;

namespace Assets.Script.Backend
{
    public partial class GameSceneBase
    {
        protected ConcurrentDictionary<string, BackendGameObject> AllGameObjectCollection { get; set; }

        public GameSceneBase()
        {
            AllGameObjectCollection = new ConcurrentDictionary<string, BackendGameObject>();
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
            PlayerInventory = new Inventory(100);
            InitializeCharacters();
        }
    }
}
