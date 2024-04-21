using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemQuickAccess : MonoBehaviour
{
    private GameItemDynamic[] quickAccessList;
    // Start is called before the first frame update
    void Start()
    {
        quickAccessList = Backend.GameLoop.quickAccessItems;
    }

    public void OnClicked(int index)
    {
        if (quickAccessList[index] != null && quickAccessList[index].CurrentCount > 0) {
            ConsumableController.Instance.consumeItem(quickAccessList[index]);
        }
    }
}
