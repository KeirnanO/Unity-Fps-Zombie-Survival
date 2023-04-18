using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkInteractable : NetworkBehaviour
{
    public string toolTip = "Hold 'E' to";

    [Command(requiresAuthority = false)]
    public virtual void Interact(uint netId) { }

    public virtual string GetTooltip()
    {
        return toolTip;
    }
}
