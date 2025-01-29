using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Network2DEnemySpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab = null;

    public LayerMask spawnableAreaLayers;

    public Vector2 minPosition;
    public Vector2 maxPosition;

    [SerializeField] float spawnTime = 10f;
    [SerializeField] float _spawnTimeoutDelta = 0f;

    int difficulty = 1;

    public override void OnStartServer()
    {
        base.OnStartServer();

        for (int i = 0; i < 70; i++)
        {
            SpawnEnemy();
        }
    }

    
    [ServerCallback]
    private void Update()
    {
        if(Time.time > _spawnTimeoutDelta)
        {
            int diff = Mathf.FloorToInt(difficulty / 10);

            if (diff > 1)
            {
                for (int i = 0; i < diff; i++)
                {
                    SpawnEnemy();
                }
            }
            else
            {
                SpawnEnemy();
            }

            _spawnTimeoutDelta = Time.time + spawnTime;
            difficulty += 5;
        }
    }
    

    [Server]
    public void SpawnEnemy()
    {
        float posX = Random.Range(minPosition.x, maxPosition.x);
        float posZ = Random.Range(minPosition.y, maxPosition.y);

        GameObject newEnemy = Instantiate(enemyPrefab, new Vector2(posX, posZ), Quaternion.identity);

        NetworkServer.Spawn(newEnemy);
    }
}
