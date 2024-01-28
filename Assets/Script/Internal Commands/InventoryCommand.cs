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
}
