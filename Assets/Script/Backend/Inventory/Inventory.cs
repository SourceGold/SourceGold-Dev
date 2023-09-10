using System;
using System.Collections.Concurrent;

#nullable enable

namespace Assets.Script.Backend
{
    public class Inventory
    {
        private int _itemMaxCount;
        private ConcurrentDictionary<string, InventoryItem> _items { get; set; }

        public Inventory(int itemMaxCount)
        {
            _items = new ConcurrentDictionary<string, InventoryItem>();
            _itemMaxCount = itemMaxCount;
        }

        public void AddItem(InventoryItem item, out InventoryItem? notAddedItem)
        {
            notAddedItem = null;
            var addedCount = item.CurrentCount;
            if (_items.TryGetValue(item.Name, out var existingItem))
            {
                addedCount = existingItem.AddItems(item.CurrentCount);
                if (addedCount != item.CurrentCount)
                {
                    notAddedItem = item.Clone();
                    notAddedItem.RemoveItems(addedCount);
                }
                return;
            }
            else if (_items.Count >= _itemMaxCount)
            {
                GameEventLogger.LogEvent($"Faied to Add item name: {item.Name}, count: {addedCount} to inventory, Inventory is full");
                EventManager.TriggerEvent(GameEventTypes.GetInventoryFullEvent);
                notAddedItem = item.Clone();
                return;
            }
            _items.TryAdd(item.Name, item);
            GameEventLogger.LogEvent($"Adding item name: {item.Name}, count: {addedCount} to inventory");
        }

        public void UseItems(InventoryItem item, out InventoryItem? updatedItem)
        {
            if (_items.TryGetValue(item.Name, out var existingItem))
            {
                if (existingItem.CurrentCount < item.CurrentCount)
                {
                    throw new Exception($"Not enough items in inventory. Requested: {item.CurrentCount}, available: {existingItem.CurrentCount}");
                }
                for (int i = 0; i < item.CurrentCount; i++)
                {
                    existingItem.UseItem();
                }
                updatedItem = existingItem;
            }
            throw new Exception($"Item {item.Name} not found in inventory");
        }

        public void RemoveItem(InventoryItem item)
        {
            if (!_items.TryRemove(item.Name, out _))
            {
                throw new Exception($"Item {item.Name} not found in inventory");
            }
        }
    }
}
