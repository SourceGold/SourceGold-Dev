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
    public List<InventoryItem> _items { get; set; }

    public Inventory(int itemMaxCount)
    {
        _items = new List<InventoryItem>();
        _itemMaxCount = itemMaxCount;
    }

    public InventoryItem GetInventoryItem(InventoryItem inventoryItem)
    {
        foreach (var existingItem in _items)
        {
            if (existingItem.id == inventoryItem.id && existingItem.level == inventoryItem.level)
                return existingItem;
        }
  
        throw new Exception($"GameItem of {inventoryItem.id}, does not found in the static library");
    }

    public int AddItem(InventoryItem inventoryItem)
    {
        foreach (var existingItem in _items)
        {
            if (existingItem.id == inventoryItem.id && existingItem.level == inventoryItem.level)
            {
                var remainingAmount = existingItem.AddItems(inventoryItem.CurrentCount);
                GameEventLogger.LogEvent($"Adding item Id: {inventoryItem.id}, plan to add {inventoryItem.CurrentCount}, " +
                    $"and {inventoryItem.CurrentCount - remainingAmount} was added to inventory");
                return remainingAmount;
            }
        }
        
        if (_items.Count >= _itemMaxCount)
        {
            GameEventLogger.LogEvent($"Faied to Add item Id: {inventoryItem}, to inventory, Inventory is full");
            EventManager.TriggerEvent(GameEventTypes.GetInventoryFullEvent);
            return inventoryItem.CurrentCount;
        } 

        _items.Add(inventoryItem);
        GameEventLogger.LogEvent($"Creating new item: {inventoryItem} added to inventory");
        return 0;
        
    }
    
    public int RemoveItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].id == inventoryItem.id && _items[i].level == inventoryItem.level)
            {
                if (_items[i].CurrentCount < inventoryItem.CurrentCount)
                {
                    throw new Exception($"Not enough items in inventory. Requested: {inventoryItem}, available: {_items[i]}");
                }
                _items[i].RemoveItems(inventoryItem.CurrentCount);
                return _items[i].CurrentCount;
            }
        }

        throw new Exception($"Item Id: {inventoryItem} not found in inventory");
    }
    

    /// <summary>
    /// This function will remove all inventory items with matching ID and Level
    /// </summary>
    /// <param name="inventoryItem">The ID and Level that you want to remove</param>
    /// <exception cref="Exception">There should be one and only one in the inventory. If not found will throw</exception>
    public void RemoveAllItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].id == inventoryItem.id && _items[i].level == inventoryItem.level)
            {
                _items.RemoveAt(i);
                return;
            }
        }
        throw new Exception($"Item {inventoryItem} not found in inventory");
    }

    /// <summary>
    /// This function will remove all inventory items with matching name
    /// </summary>
    /// <param name="itemID"></param>
    public void RemoveAllItem(string itemID)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].id == itemID)
            {
                _items.RemoveAt(i);
            }
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

