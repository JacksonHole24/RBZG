using System.Collections;
using UnityEngine;

public class ZombieSpawner1 : MonoBehaviour
{
    [Tooltip("The zombie prefab to spawn.")]
    public GameObject zombiePrefab;
    [Tooltip("The radius of the sphere cast.")]
    public float spawnRadius = 10f;
    [Tooltip("The total number of zombies to spawn.")]
    public int spawnCount = 5;
    [Tooltip("The minimum time between each zombie spawn.")]
    public float minSpawnTime = 1f;
    [Tooltip("The maximum time between each zombie spawn.")]
    public float maxSpawnTime = 5f;

    private int zombiesSpawned = 0;
    private int emptyCount = 0;
    private Collider[] colliders;

    private void Start()
    {
        // Get all the empty game objects around the player within the spawn radius.
        colliders = Physics.OverlapSphere(transform.position, spawnRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("ZSpawm"))
            {
                emptyCount++;
            }
        }

        // Start spawning zombies at random empty game objects.
        StartCoroutine(SpawnZombies());
    }

    private IEnumerator SpawnZombies()
    {
        while (zombiesSpawned < spawnCount)
        {
            if (emptyCount > 0)
            {
                int randomIndex = Random.Range(0, emptyCount);
                int currentIndex = 0;
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.CompareTag("ZSpawm"))
                    {
                        if (currentIndex == randomIndex)
                        {
                            Instantiate(zombiePrefab, collider.transform.position, Quaternion.identity);
                            emptyCount--;
                            zombiesSpawned++;
                            break;
                        }
                        currentIndex++;
                    }
                }
            }

            // Wait for a random time before spawning the next zombie.
            float spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the spawn radius.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
