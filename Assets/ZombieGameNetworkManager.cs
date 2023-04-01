using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGameNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        ChatBox.instance.SendServerMessage(conn.identity.transform.GetComponent<NetworkPlayerController>().playerName + " Has Disconnected");
        base.OnServerDisconnect(conn);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        //ChatBox.instance.SendServerMessage(conn.identity.transform.GetComponent<NetworkPlayerController>().playerName + " Has ");
        base.OnServerConnect(conn);
    }
}
