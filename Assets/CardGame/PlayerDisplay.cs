using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerDisplay : MonoBehaviour, IPointerClickHandler
{
    public string Name;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public Image[] manaIcons;

    public Color[] manaIconColours;

    public int index;
    
    public void SetPlayerDisplay(string name, Health health, Mana mana)
    {
        nameText.text = name;
        healthText.text = health.GetHealth() + " / " + health.GetMaxHealth();
        
        foreach(Image icon in manaIcons)
        {
            icon.enabled = false;
        }

        if (mana == null)
            return;

        mana.Sort();

        for(int i = 0; i < Mathf.Clamp(mana.manaPool.Count, 0, manaIcons.Length); i++)
        {
            manaIcons[i].enabled = true;
            manaIcons[i].color = manaIconColours[(int)mana.manaPool[i]];
        }
    }

    public void SetPlayerCardDisplay(int cardID, int targetID)
    {
        print(Name + " is using " + cardID + " on " + targetID);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<FightPlayer>().SelectTarget(index);
    }
}
