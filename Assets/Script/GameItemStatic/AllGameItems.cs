using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New GameItem", menuName = "GameItem/List of all items")]
public class AllGameItems : ScriptableObject
{
    public List<GameItem> gameItems;
}