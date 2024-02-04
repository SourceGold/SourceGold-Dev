using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class InventoryCommand 
{
    [MenuItem("Internal Controll/Read all Game Item")]
    public static void readGameItem() {
        AllGameItems allGameItems = Resources.Load<AllGameItems>("GameItems/AvaliableItems");
        Debug.Log(allGameItems);
        foreach (GameItem item in allGameItems.gameItems)
        {
           Debug.Log(item.name);
        }
    }

    [MenuItem("Internal Controll/Log Selected Transform Name")]
    static void LogSelectedTransformName()
    {
        Debug.Log("Selected Transform is on " + Selection.activeTransform.gameObject.name + ".");
    }

    [MenuItem("Internal Controll/Add To Inventory")]
    static void AddToInventory()
    {
        for (int i = 0; i < 3; i++)
        {
            InventoryItem aItem = new InventoryItem($"Cystal", level: i, currentCount: i);
            Backend.GameLoop.GetInventory().AddItem(aItem);
        }
    }

    [MenuItem("Internal Controll/Print Inventory")]
    static void PrintToInventory()
    {
        Debug.Log(Backend.GameLoop.GetInventory());
    }
}
