using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkChest : NetworkInteractable
{
    public GameObject[] items;

    float drops = 1;

    float luck = 0;

    public override void OnStartServer()
    {
        luck = Random.Range(0, 1);
        drops = Random.Range(1, 4);

        base.OnStartServer();
    }

    [Command(requiresAuthority = false)]
    public override void Interact(uint netId)
    {
        for (int i = 0; i < drops; i++)
        {
            float gunNum = Random.Range(0f, items.Length) + luck;
            int index = Mathf.FloorToInt(gunNum);

            if (index > items.Length - 1)
                index = items.Length - 1;

            GameObject newItem = Instantiate(items[index], transform.position, Quaternion.AngleAxis(Random.Range(0f, 360f),Vector3.up));
            NetworkServer.Spawn(newItem);
        }
        NetworkServer.UnSpawn(gameObject);

        RpcInteract();
        Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcInteract()
    {
        Destroy(gameObject);
    }
}
