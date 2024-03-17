using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

    static int number = 0;
    [MenuItem("Internal Controll/Add To Inventory")]
    static void AddToInventory()
    {
        for (int i = number; i < number + 1; i++)
        {
            GameItemDynamic aItem = new GameItemDynamic($"Cystal", level: i, currentCount: i);
            GameItemDynamic bItem = new GameItemDynamic($"BigSward", level: i, currentCount: i);
            GameItemDynamic cItem = new GameItemDynamic($"Potion", level: i, currentCount: i);
            Backend.GameLoop.GetInventory().AddItem(aItem);
            Backend.GameLoop.GetInventory().AddItem(bItem);
            Backend.GameLoop.GetInventory().AddItem(cItem);
        }
        number += 3;
    }

    [MenuItem("Internal Controll/Print Inventory")]
    static void PrintToInventory()
    {
        Debug.Log(Backend.GameLoop.GetInventory());
    }
}
