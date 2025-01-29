using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemDrop : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public FactoryItem item;
    public int amount;    

    [Server]
    public void ServerSetItem(FactoryItem _item, int _amount)
    {
        item = _item;
        amount = _amount;

        RpcSetItem(FactoryItemDatabase.instance.GetItemID(_item), amount);
    }

    [ClientRpc]
    public void RpcSetItem(int itemID, int _amount)
    {
        item = FactoryItemDatabase.instance.itemDatabase[itemID];
        amount = _amount;

        spriteRenderer.sprite = item.inventoryIcon;
    }

    [Command (requiresAuthority = false)]
    public void CommandPickUpItemCallBack()
    {
        DestroyObject();
    }

    [Server]
    public void DestroyObject()
    {
        NetworkServer.Destroy(gameObject);
    }
}
