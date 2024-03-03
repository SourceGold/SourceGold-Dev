#nullable enable

using System.Collections.Generic;

namespace Assets.Script.Backend
{
    public partial class GameSceneBase
    {
        public Inventory PlayerInventory { get; set; }
        public List<int> quickAccessItems { get; set; }

        public void InitializeStageInventory(Inventory inventory)
        {
            PlayerInventory = inventory;

        }
        public void InitializeStageInventory()
        {
            PlayerInventory = new Inventory(100);
            quickAccessItems = new List<int>(new int[4] {-1, -1, -1, -1});

        }
        public Inventory GetInventory()
        {
            return PlayerInventory;
        }
    }
}
