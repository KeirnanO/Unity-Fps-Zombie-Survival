using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class NetworkChestSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject chestPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer() => SpawnChests();

    [Server]
    public void SpawnChests()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject chestInstance = Instantiate(chestPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(chestInstance);
        }
    }
}
