using RBZG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner1 : MonoBehaviour
{
    [Tooltip("The zombie prefab to spawn.")]
    public List<Zombie> zombies;
    [Tooltip("The radius of the sphere cast.")]
    public float spawnRadius = 10f;
    [Tooltip("The minimum number of zombies to spawn per tick.")]
    public int minSpawnCount = 1;
    [Tooltip("The maximum number of zombies to spawn per tick.")]
    public int maxSpawnCount = 3;
    [Tooltip("The minimum time between each zombie spawn.")]
    public float minSpawnTime = 1f;
    [Tooltip("The maximum time between each zombie spawn.")]
    public float maxSpawnTime = 5f;
    [Tooltip("The total number of zombies to spawn.")]
    public int totalZombiesToSpawn = 10;

    private int emptyCount = 0;
    private Collider[] colliders;
    private int spawnedZombies = 0;
    private float spawnTimer = 0f;
    private int currentRound = 1;
    private int zombiesToSpawnThisRound = 0;
    [HideInInspector] public int zombiesKilledThisRound = 0;

    private HUDManager hudManager;

    private void Awake()
    {
        hudManager = FindObjectOfType<HUDManager>();
    }

    private void Start()
    {
        zombiesToSpawnThisRound = totalZombiesToSpawn;
    }

    private void Update()
    {
        if (zombiesKilledThisRound >= zombiesToSpawnThisRound)
        {
            currentRound++;
            hudManager.UpdateRoundText(currentRound);
            zombiesKilledThisRound = 0;
            spawnedZombies = 0;
            zombiesToSpawnThisRound = totalZombiesToSpawn * currentRound;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            if (emptyCount > 0 && spawnedZombies < zombiesToSpawnThisRound)
            {
                int spawnCount = Random.Range(minSpawnCount, maxSpawnCount);

                if(spawnCount > zombiesToSpawnThisRound - spawnedZombies) 
                {
                    spawnCount = Random.Range(minSpawnCount, zombiesToSpawnThisRound - spawnedZombies);
                }

                for (int i = 0; i < spawnCount; i++)
                {
                    int randomIndex = Random.Range(0, emptyCount);
                    int currentIndex = 0;
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.CompareTag("ZSpawn"))
                        {
                            if (currentIndex == randomIndex)
                            {
                                int ran = Random.Range(0, zombies.Count);
                                GameObject instantiated = Instantiate(zombies[ran].prefab, collider.transform.position, Quaternion.identity);
                                instantiated.GetComponent<ZombieAI>().zombieStats = zombies[ran];
                                spawnedZombies++;
                                Debug.Log(spawnedZombies);
                                break;
                            }
                            currentIndex++;
                        }
                    }
                }
            }

            // Wait for a random time before spawning the next batch of zombies.
            float spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            spawnTimer = spawnTime;
        }

        // Update the colliders array to get the empty game objects around the player within the spawn radius.
        colliders = Physics.OverlapSphere(transform.position, spawnRadius);
        emptyCount = 0;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "ZSpawn")
            {
                emptyCount++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the spawn radius.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
