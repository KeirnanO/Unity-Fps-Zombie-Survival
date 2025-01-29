using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using UnityEngine.AI;

public class NetworkEnemySpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject zombiePrefab = null;

    public LayerMask spawnableAreaLayers;

    public Vector2 minPosition;
    public Vector2 maxPosition;

    float spawnTime = 30f;
    [SerializeField] float _spawnTimeoutDelta = 0f;

    int difficulty = 1;

    public override void OnStartServer()
    {
        base.OnStartServer();

        for(int i = 0; i < 70; i++)
        {
            SpawnZombie();
        }
    }

    /*
    [ServerCallback]
    private void Update()
    {
        if(Time.time > _spawnTimeoutDelta)
        {
            for (int i = 0; i < Mathf.Min(difficulty * 10, 30); i++)
            {
                SpawnZombie();                
            }

            _spawnTimeoutDelta = Time.time + spawnTime;
            difficulty++;
        }
    }
    */

    [Server]
    public void SpawnZombie()
    {
        float posX = Random.Range(minPosition.x, maxPosition.x);
        float posZ = Random.Range(minPosition.y, maxPosition.y);

        if (Physics.Raycast(new Vector3(posX, 100f, posZ), Vector3.down, out RaycastHit hit, 200f, spawnableAreaLayers))
        {
            NavMeshHit navMeshHit;
            var result = NavMesh.SamplePosition(hit.point, out navMeshHit, 1, spawnableAreaLayers);

            print("Spawning Zombie");

            if (result)
            {
                GameObject newZombie = Instantiate(zombiePrefab, navMeshHit.position, Quaternion.identity);

                NetworkServer.Spawn(newZombie);
            }
            else
            {
                SpawnZombie();
                return;
            }
        }
    }
}