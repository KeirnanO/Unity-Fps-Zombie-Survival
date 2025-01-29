using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private InventoryMenuWindow inventoryWindow;
    [SerializeField] public Image inventorySlotIcon;
    [SerializeField] public TextMeshProUGUI itemAmountText;
    
    public FactoryItem m_item = null;

    public void SetItem(FactoryItem item, int amount)
    {
        m_item = item;

        if (item == null)
        {
            inventorySlotIcon.enabled = false;
            itemAmountText.text = "";
            return;
        }

        inventorySlotIcon.enabled = true;
        inventorySlotIcon.sprite = item.inventoryIcon;


        if (item.isStackable || amount > 1)
        {
            itemAmountText.text = amount.ToString();
            return;
        }
        else
        {
            itemAmountText.text = "";
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inventoryWindow.SelectSlot(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
