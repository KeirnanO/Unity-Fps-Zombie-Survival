using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/CraftingRecipe", order = 1)]
public class CraftingRecipe : ScriptableObject
{
    public FactoryItem[] recipeItems;
    public int[] recipeAmounts;

    public FactoryItem producedItem;
    public int producedAmount;
}
