using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameItemType
{
    Consumerable,
    Weapons,
    Quest,
    EnemyDrop,
    Default
}

[CreateAssetMenu(fileName = "New GameItem", menuName = "GameItem/GenericItem")]
public class GameItem : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public GameItemType type;
    public int max_level;
    public int maximum_count;

    public GameItemDescription englishDescription;
    public GameItemDescription chineseDescription;
}
