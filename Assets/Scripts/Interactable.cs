using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string toolTip = "Hold 'E' to";


    public virtual void Interact()
    {

    }

    public virtual string GetTooltip()
    {
        return toolTip;
    }
}
