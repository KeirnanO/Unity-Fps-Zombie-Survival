using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] public FactoryItem[] itemInventoryKeys;
    [SerializeField] public int[] itemInventoryValues;
    public void SwapItems(int slot1, int slot2)
    {
        int tempAmount = 0;
        FactoryItem tempItemp = null;

        tempItemp = itemInventoryKeys[slot1];
        tempAmount = itemInventoryValues[slot1];

        itemInventoryKeys[slot1] = itemInventoryKeys[slot2];
        itemInventoryValues[slot1] = itemInventoryValues[slot2];

        itemInventoryKeys[slot2] = tempItemp;
        itemInventoryValues[slot2] = tempAmount;
    }

    public void AddItem(FactoryItem item, int amount)
    {
        if (item.isStackable)
        {
            for(int i = 0; i < itemInventoryKeys.Length; i++)
            {
                if (itemInventoryKeys[i] == item)
                {
                    itemInventoryValues[i] += amount;
                    return;
                }
            }
        }

        if (IsFull())
            return;

        for(int i = 0; i < itemInventoryKeys.Length; i++)
        {
            if (itemInventoryKeys[i] == null)
            {
                itemInventoryKeys[i] = item;
                itemInventoryValues[i] = amount;
                return;
            }    
        }
    }

    public bool IsFull()
    {
        foreach(FactoryItem item in itemInventoryKeys)
        {
            if (item == null)
                return false;
        }

        return true;
    }

    public bool ContainsRecipeItems(CraftingRecipe recipe)
    {
        for (int recipeItemIndex = 0; recipeItemIndex < recipe.recipeItems.Length; recipeItemIndex++)
        {
            bool found = false;

            for(int playerIventoryIndex = 0; playerIventoryIndex < itemInventoryKeys.Length; playerIventoryIndex++)
            {
                if(itemInventoryKeys[playerIventoryIndex] == recipe.recipeItems[recipeItemIndex])
                {
                    if (itemInventoryValues[playerIventoryIndex] >= recipe.recipeAmounts[recipeItemIndex])
                    {
                        found = true;
                        break;
                    }
                    else
                    {
                        found = false;
                        break;
                    }
                }
            }

            if (!found)
                return false;
        }

        return true;
    }

    public void RemoveRecipeItems(CraftingRecipe recipe)
    {
        for (int recipeItemIndex = 0; recipeItemIndex < recipe.recipeItems.Length; recipeItemIndex++)
        {
            for (int playerIventoryIndex = 0; playerIventoryIndex < itemInventoryKeys.Length; playerIventoryIndex++)
            {
                if (itemInventoryKeys[playerIventoryIndex] == recipe.recipeItems[recipeItemIndex])
                {
                    itemInventoryValues[playerIventoryIndex] -= recipe.recipeAmounts[recipeItemIndex];
                    
                    if(itemInventoryValues[playerIventoryIndex] <= 0)
                    {
                        itemInventoryValues[playerIventoryIndex] = 0;
                        itemInventoryKeys[playerIventoryIndex] = null;
                    }
                }
            }
        }
    }

    public bool ContainsStackableItem(FactoryItem item)
    {
        if (!item.isStackable)
            return false;

        foreach(FactoryItem item2 in itemInventoryKeys)
        {
            if (item == item2)
                return true;
        }

        return false;
    }
}
