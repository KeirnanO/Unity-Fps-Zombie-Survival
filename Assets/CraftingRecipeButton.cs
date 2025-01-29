using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeButton : MonoBehaviour
{
    [SerializeField] private CraftingRecipe recipe;
    [SerializeField] private InventoryMenuWindow inventory;

    [SerializeField] private Image producedItemIcon;
    [SerializeField] private TextMeshProUGUI producedNameText;
                     
    [SerializeField] private Image[] itemCostIcons;
    [SerializeField] private TextMeshProUGUI[] itemAmountTexts;

    public void SetRecipe(CraftingRecipe recipe)
    {
        this.recipe = recipe;

        producedItemIcon.sprite = recipe.producedItem.inventoryIcon;
        producedNameText.text = recipe.producedItem.itemName;

        for(int itemCostIconIndex = 0; itemCostIconIndex < itemCostIcons.Length; itemCostIconIndex++)
        {
            if(itemCostIconIndex >= recipe.recipeItems.Length)
            {
                itemCostIcons[itemCostIconIndex].gameObject.SetActive(false);
                continue;
            }

            itemCostIcons[itemCostIconIndex].sprite = recipe.recipeItems[itemCostIconIndex].inventoryIcon;
            itemAmountTexts[itemCostIconIndex].text = recipe.recipeAmounts[itemCostIconIndex].ToString();
        }
    }

    public void CraftRecipe()
    {
        inventory.CraftRecipe(recipe);
    }
}
