using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPickUp : NetworkInteractable
{
    public int weaponIndex;

    [Command(requiresAuthority = false)]
    public override void Interact(uint netId)
    {
        NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity identity);

        NetworkLoadout playerLoadout = identity.GetComponent<NetworkLoadout>();

        playerLoadout.GiveGun(weaponIndex);
        RpcInteract();

        Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcInteract()
    {
        Destroy(gameObject);
    }
}
