using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuWindow : MenuWindow
{
    //This script will handle the visualization of the players inventory
    //It does not track the items that the player currently has

    public static InventoryMenuWindow instance;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private CraftingRecipeButton[] craftingRecipeButtons;
    [SerializeField] private CraftingRecipe[] knownRecipes;
    [SerializeField] private int recipePage = 0;

    private InventorySlot selectedSlot;
    private Vector3 originalMousePosition;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (playerInventory == null)
            return;

        UpdateInventory();

        if (selectedSlot == null)
            return;

        selectedSlot.inventorySlotIcon.rectTransform.localPosition = Input.mousePosition - originalMousePosition;
    }

    public void SelectSlot(InventorySlot slot)
    {
        if(selectedSlot == null)
        {
            if (slot.m_item == null)
                return;

            selectedSlot = slot;
            originalMousePosition = Input.mousePosition;
            return;
        }

        if (slot == selectedSlot)
        {
            selectedSlot.inventorySlotIcon.rectTransform.localPosition = Vector2.zero;
            selectedSlot = null;
            return;
        }

        int ogIndex = 0;
        int newIndex = 0;

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == selectedSlot)
                ogIndex = i;

            if (inventorySlots[i] == slot)
                newIndex = i;
        }

        playerInventory.SwapItems(ogIndex, newIndex);

        selectedSlot.inventorySlotIcon.rectTransform.localPosition = Vector2.zero;
        slot.inventorySlotIcon.rectTransform.localPosition = Vector2.zero;

        selectedSlot = null;
    }

    public void OpenInventory()
    {
        if(toolbar.gameObject.activeSelf)
        {
            CloseInventory();
            return;
        }

        OpenMenu();
        UpdateCraftingMenu();
    }

    public void CloseInventory()
    {
        if(selectedSlot != null)
            selectedSlot.inventorySlotIcon.rectTransform.localPosition = Vector2.zero;

        CloseMenu();
    }

    public void SetInventory(Inventory inventory)
    {
        playerInventory = inventory;
    }

    void UpdateInventory()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetItem(playerInventory.itemInventoryKeys[i], playerInventory.itemInventoryValues[i]);
        }
    }

    void UpdateCraftingMenu()
    {
        for(int recipeButtonIndex = 0; recipeButtonIndex < craftingRecipeButtons.Length; recipeButtonIndex++)
        {
            if(recipeButtonIndex + (recipePage * 12) >= knownRecipes.Length)
            {
                craftingRecipeButtons[recipeButtonIndex].gameObject.SetActive(false);
                continue;
            }

            craftingRecipeButtons[recipeButtonIndex].gameObject.SetActive(true);
            craftingRecipeButtons[recipeButtonIndex].SetRecipe(knownRecipes[recipeButtonIndex + (recipePage * 12)]);
        }
    }

    public void CraftRecipe(CraftingRecipe recipe)
    {
        if (playerInventory.ContainsRecipeItems(recipe))
        {
            playerInventory.RemoveRecipeItems(recipe);
            playerInventory.AddItem(recipe.producedItem, recipe.producedAmount);
        }
    }
}
