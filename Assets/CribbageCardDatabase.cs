using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CribbageCardDatabase : MonoBehaviour
{
    public static CribbageCardDatabase instance;

    private void Awake()
    {
        instance = this;
    }

    public CribbageCard[] CardDatabase;
    public CribbageCard[] CardBackDatabase;
}
