using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    public Image border;
    public Image image;
    public int index;

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<InventoryMenuController>().RemoveCard(index);
    }
}
