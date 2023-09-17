using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Assets.Script.Backend
{
    public class Inventory
    {
        private int _itemMaxCount;
        private ConcurrentDictionary<int, InventoryItem> _items { get; set; }

        public Inventory(int itemMaxCount)
        {
            _items = new ConcurrentDictionary<int, InventoryItem>();
            _itemMaxCount = itemMaxCount;
        }

        public List<(int id, int count)> GetInventoryItemList()
        {
            return _items.Select(x => (x.Key, x.Value.CurrentCount)).ToList();
        }

        public void AddItem(int itemId, int itemAddCount, out int? itemNotAddedCount)
        {
            itemNotAddedCount = null;
            var addedCount = itemAddCount;
            if (_items.TryGetValue(itemId, out var existingItem))
            {
                addedCount = existingItem.AddItems(itemAddCount);
                if (addedCount != itemAddCount)
                {
                    itemNotAddedCount = itemAddCount - addedCount;
                }
                GameEventLogger.LogEvent($"Adding item Id: {itemId}, count: {addedCount} to inventory");
                return;
            }
            else if (_items.Count >= _itemMaxCount)
            {
                GameEventLogger.LogEvent($"Faied to Add item Id: {itemId}, count: {addedCount} to inventory, Inventory is full");
                EventManager.TriggerEvent(GameEventTypes.GetInventoryFullEvent);
                itemNotAddedCount = itemAddCount;
                return;
            }
            var item = LoadInventoryItem(itemId, itemAddCount);
            _items.TryAdd(itemId, item);
            GameEventLogger.LogEvent($"Adding item Id: {itemId}, count: {addedCount} to inventory");
        }

        public void RemoveItem(int itemId, int itemUseCount, bool isUseItem, out int? itemUpdatedCount)
        {
            if (_items.TryGetValue(itemId, out var existingItem))
            {
                if (existingItem.CurrentCount < itemUseCount)
                {
                    throw new Exception($"Not enough items in inventory. Requested: {itemUseCount}, available: {existingItem.CurrentCount}");
                }
                if (isUseItem)
                {
                    for (int i = 0; i < itemUseCount; i++)
                    {
                        existingItem.UseItem();
                    }
                }
                itemUpdatedCount = existingItem.CurrentCount;
            }
            else
            {
                throw new Exception($"Item Id: {itemId} not found in inventory");
            }
        }

        public void RemoveAllItem(int itemId)
        {
            if (!_items.TryRemove(itemId, out _))
            {
                throw new Exception($"Item {itemId} not found in inventory");
            }
        }

        protected InventoryItem LoadInventoryItem(int itemId, int count)
        {
            var item = new InventoryItem(itemId, count);
            return item;
        }
    }
}
