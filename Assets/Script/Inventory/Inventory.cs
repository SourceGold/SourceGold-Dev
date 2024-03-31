using Assets.Script.Backend;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Inventory
{
    private int _itemMaxCount;
    public List<GameItemDynamic> _items { get; set; }

    public Inventory(int itemMaxCount)
    {
        _items = new List<GameItemDynamic>();
        _itemMaxCount = itemMaxCount;
    }

    public GameItemDynamic GetInventoryItem(GameItemDynamic inventoryItem)
    {
        var current_item = _items.Find(e => e.Equals(inventoryItem));
        if (current_item != null)
        {
            return current_item;
        }
        else { 
            throw new Exception($"GameItem of {inventoryItem.id}, does not found in the static library");
        }
    }


    public int AddItem(GameItemDynamic inventoryItem)
    {
        var current_item = _items.Find(e => e.Equals(inventoryItem));
        if (current_item != null)
        {
            var remainingAmount = current_item.AddItems(inventoryItem.CurrentCount);
            GameEventLogger.LogEvent($"Adding item Id: {inventoryItem.id}, plan to add {inventoryItem.CurrentCount}, " +
                $"and {inventoryItem.CurrentCount - remainingAmount} was added to inventory");
            EventManager.TriggerEvent(GameEventTypes.InventoryChangeEvent);
            return remainingAmount;
        }
        else
        {
            if (_items.Count >= _itemMaxCount)
            {
                GameEventLogger.LogEvent($"Faied to Add item Id: {inventoryItem}, to inventory, Inventory is full");
                EventManager.TriggerEvent(GameEventTypes.GetInventoryFullEvent);
                return inventoryItem.CurrentCount;
            }
            _items.Add(inventoryItem);
            GameEventLogger.LogEvent($"Creating new item: {inventoryItem} added to inventory");
            EventManager.TriggerEvent(GameEventTypes.InventoryChangeEvent);
            return 0;
        }
    }
    
    public int RemoveItem(GameItemDynamic inventoryItem, int count)
    {

        var current_item = _items.Find(e => e.Equals(inventoryItem));
        if (current_item != null)
        {
            if (current_item.CurrentCount < count)
            {
                throw new Exception($"Not enough items in inventory. Requested: {count}, available: {current_item.CurrentCount}");
            }
            else if (count < 0 || current_item.CurrentCount == count)
            {
                current_item.RemoveItems(count);
                _items.Remove(current_item);
            } 
            else
            {
                current_item.RemoveItems(count);

            }
            EventManager.TriggerEvent(GameEventTypes.InventoryChangeEvent);
            return current_item.CurrentCount;
        } else
        {
            throw new Exception($"Item Id: {inventoryItem.id} not found in inventory");
        }
    }

    public override string ToString()
    {
        string toString = "";
        foreach (var item in _items)
        {
            toString += item.ToString();
            toString += "\n";
        }
        return toString;
    }
}

