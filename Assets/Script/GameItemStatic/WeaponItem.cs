using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "GameItem/Weapon")]
public class WeaponItem : GameItem
{
    public void Awake()
    {
        type = GameItemType.Weapons;
    }
}
