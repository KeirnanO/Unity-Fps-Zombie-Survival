using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkCamera : MonoBehaviour
{
    public static CustomNetworkCamera instance;

    public Transform CameraTarget;

    private void Awake()
    {
        instance = this;
    }
}
