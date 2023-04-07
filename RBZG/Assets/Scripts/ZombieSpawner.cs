using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Tooltip("The zombie prefab to spawn.")]
    public GameObject zombiePrefab;
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

    private void Start()
    {
        // Get all the empty game objects around the player within the spawn radius.
        colliders = Physics.OverlapSphere(transform.position, spawnRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "ZSpawn")
            {
                emptyCount++;
            }
        }
    }

    private void Update()
    {
        if (spawnedZombies < totalZombiesToSpawn)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                if (emptyCount > 0)
                {
                    int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
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
                                    GameObject instantiated = Instantiate(zombiePrefab, collider.transform.position, Quaternion.identity);
                                    emptyCount--;
                                    spawnedZombies++;
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
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the spawn radius.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
