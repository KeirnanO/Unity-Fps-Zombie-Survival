using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CribbageBoard : MonoBehaviour
{
    public static CribbageBoard instance;

    private void Awake()
    {
        instance = this;
    }

    public CribbageLane[] lanes;
}
