using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType 
{ 
    None = 0,
    head = 1,
    body = 2,
    legs = 3,
    boots = 4,
    Deck = 5,
    Wand = 6
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemObject : ScriptableObject
{
    public ItemType itemType;
    public CardScriptableObject[] bonusCards;

    //stats bonus
}
