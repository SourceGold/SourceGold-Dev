using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameItem", menuName = "GameItem/ItemDescription")]
public class GameItemDescription : ScriptableObject
{
    public string displayName;
    [TextArea(20, 50)]
    public string displayDescription;
}
