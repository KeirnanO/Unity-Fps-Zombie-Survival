using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase instance;

    public List<CardScriptableObject> cardList = new List<CardScriptableObject>();

    private void Awake()
    {
        instance = this;
    }
}
