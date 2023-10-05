using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public EquipmentSlot head;
    public EquipmentSlot body;
    public EquipmentSlot legs;
    public EquipmentSlot boots;
    public EquipmentSlot wand;
    public EquipmentSlot deck;


    public CardScriptableObject GetAttackCard()
    {
        return wand.itemObject.bonusCards[0];
    }

    public List<CardScriptableObject> GetBonusCards()
    {
        List<CardScriptableObject> list = new List<CardScriptableObject>();
        EquipmentSlot[] slots = { head, body, legs, boots, wand, deck };

        foreach (var slot in slots)
        {
            if(slot != null)
            {
                list.AddRange(slot.itemObject.bonusCards);
            }
        }

        
        return list;
    }

}
