using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject zombiePrefab;

    public Transform[] spawnPoints;

    public int round = 0;
    public List<GameObject> spawnedZombies;

    int zombiesToSpawn;
    int zombiesSpawnedThisRound;

    public float spawnTime = 5;
    public float timeToSpawn = 0;

    public int spawnNum = 0;

    bool spawning = false;


    private void Start()
    {
        StartRound();
    }

    private void Update()
    {
        if(spawning)
            Spawn();


        if(!spawning)
        {
            if(zombiesSpawnedThisRound >= zombiesToSpawn && spawnedZombies.Count == 0)
            {
                StartRound();
            }
        }

        foreach(var i in spawnedZombies)
        {
            if(i == null)
            {
                spawnedZombies.Remove(i);
                break;
            }
        }
    }

    private void StartRound()
    {
        round++;
        spawnNum = 0;
        zombiesSpawnedThisRound = 0;
        spawnedZombies.Clear();

        zombiesToSpawn = round * 8;
        spawnTime = 5 / round;

        spawning = true;
    }

    void Spawn()
    {
        if (Time.time > timeToSpawn)
        {
            timeToSpawn = Time.time + spawnTime;

            spawnedZombies.Add(Instantiate(zombiePrefab, spawnPoints[spawnNum].position, Quaternion.identity));

            spawnNum = (spawnNum + 1) % spawnPoints.Length;
            zombiesSpawnedThisRound++;

            if (zombiesSpawnedThisRound >= zombiesToSpawn) 
                spawning = false;
        }
    }
}
