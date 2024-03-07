using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemsStaticManager : Singleton<GameItemsStaticManager>
{
    public Dictionary<string, GameItem> gameItemsStatic = new Dictionary<string, GameItem>();
    // Start is called before the first frame update
    void Start()
    {
        AllGameItems allGameItems = Resources.Load<AllGameItems>("GameItems/AvaliableItems");
        Debug.Log(allGameItems);
        foreach (GameItem item in allGameItems.gameItems)
        {
            print(item.itemName);

            gameItemsStatic[item.itemName] = item;
        }
    }

    public GameItem GetGameItem(string name)
    {
        if (gameItemsStatic.TryGetValue(name, out var existingItem)) {
            return existingItem;
        } else
        {
            throw new Exception($"GameItem of {name}, does not found in the static library");
        }
    }
}
