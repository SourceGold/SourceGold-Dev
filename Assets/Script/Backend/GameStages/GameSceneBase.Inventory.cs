#nullable enable

using System.Collections.Generic;

namespace Assets.Script.Backend
{
    public partial class GameSceneBase
    {
        public void InitializeStageInventory(Inventory inventory)
        {
            PlayerInventory = inventory;
        }

        public Inventory GetInventory()
        {
            return PlayerInventory;
        }

        public List<(int id, int count)> GetInventoryItemList()
        {
            return PlayerInventory.GetInventoryItemList();
        }

        public void AddItem(int itemId, int itemAddCount, out int? itemNotAddedCount)
        {
            PlayerInventory.AddItem(itemId, itemAddCount, out itemNotAddedCount);
        }

        public void RemoveItem(int itemId, int itemAddCount, out int? itemUpdatedCount)
        {
            PlayerInventory.RemoveItem(itemId, itemAddCount, isUseItem: false, out itemUpdatedCount);
        }

        public void UseItem(int itemId, int itemAddCount, out int? itemUpdatedCount)
        {
            PlayerInventory.RemoveItem(itemId, itemAddCount, isUseItem: true, out itemUpdatedCount);
        }

        public void RemoveAllItem(int itemId)
        {
            PlayerInventory.RemoveAllItem(itemId);
        }
    }
}
