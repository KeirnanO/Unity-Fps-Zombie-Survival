using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWindow : MonoBehaviour
{
    [SerializeField] private string windowName;
    [SerializeField] protected Toolbar toolbar;

    public void OpenMenu()
    {
        toolbar.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        toolbar.gameObject.SetActive(false);
    }


    public string GetName()
    {
        return windowName;
    }
}
