#nullable enable

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

        public void AddItem(InventoryItem item, out InventoryItem? notAddedItem)
        {
            PlayerInventory.AddItem(item, out notAddedItem);
        }

        public void RemoveItem(InventoryItem item)
        {
            PlayerInventory.RemoveItem(item);
        }

        public void UseItems(InventoryItem item, out InventoryItem? updatedItem)
        {
            PlayerInventory.UseItems(item, out updatedItem);
        }
    }
}
