using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItemDatabase : MonoBehaviour
{
    public static FactoryItemDatabase instance;

    private void Awake()
    {
        instance = this;
    }

    //TODO:: ADD CATEGORIES OF ITEMS TO REDUCE THE TIME IT TAKES TO GRAB THE ITEM ID

    public FactoryItem[] itemDatabase;

    public int GetItemID(FactoryItem item)
    {
        for(int i = 0; i < itemDatabase.Length; i++)
        {
            if(itemDatabase[i] == item)
                return i;
        }

        return -1;
    }
}
