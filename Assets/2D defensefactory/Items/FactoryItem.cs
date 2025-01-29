using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/FactoryItem", order = 1)]
public class FactoryItem : ScriptableObject
{
    public Sprite inventoryIcon = null;
    public ItemDrop droppedItemObject;

    public bool isStackable = true;
    public int stackSize = 50;

    public string itemName;

    public int ItemID
    {
        get
        {
            return FactoryItemDatabase.instance.GetItemID(this);
        }
    }
}
