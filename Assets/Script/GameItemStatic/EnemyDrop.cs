using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "GameItem/EnemyDrop")]
public class EnemyDrop : GameItem
{
    public void Awake()
    {
        type = GameItemType.EnemyDrop;
    }
}