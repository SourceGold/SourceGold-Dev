#nullable enable

using System.Collections.Generic;

namespace Assets.Script.Backend
{
    public partial class GameSceneBase
    {
        public Inventory PlayerInventory { get; set; }

        public void InitializeStageInventory(Inventory inventory)
        {
            PlayerInventory = inventory;
        }

        public Inventory GetInventory()
        {
            return PlayerInventory;
        }
    }
}
