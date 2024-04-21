#nullable enable

using System.Collections.Generic;

namespace Assets.Script.Backend
{
    public partial class GameSceneBase
    {
        public Inventory PlayerInventory { get; set; }
        public GameItemDynamic[] quickAccessItems { get; set; }

        public void InitializeStageInventory(Inventory inventory)
        {
            PlayerInventory = inventory;

        }
        public void InitializeStageInventory()
        {
            PlayerInventory = new Inventory(100);
            quickAccessItems = new GameItemDynamic[4];

        }
        public Inventory GetInventory()
        {
            return PlayerInventory;
        }
    }
}
