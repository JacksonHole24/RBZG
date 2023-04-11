using RBZG;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
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
    [Tooltip("The time to wait between rounds.")]
    public float roundDelay = 5f;

    private int emptyCount = 0;
    private Collider[] colliders;
    private int spawnedZombies = 0;
    private float spawnTimer = 0f;
    private int currentRound = 1;
    private int zombiesToSpawnThisRound = 0;
    [HideInInspector] public int zombiesKilledThisRound = 0;
    private bool roundHasChanged = false;
    private int extraZombiesPerRound = 0;

    private HUDManager hudManager;

    private void Awake()
    {
        hudManager = FindObjectOfType<HUDManager>();
    }

    private void Start()
    {
        StartCoroutine(StartSpawning());

        zombiesToSpawnThisRound = totalZombiesToSpawn;
        spawnedZombies = totalZombiesToSpawn;
    }

    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(roundDelay);
        switch (currentRound)
        {
            case 2:
                extraZombiesPerRound = 5; break;
            case 5:
                extraZombiesPerRound = 6; break;
            case 7:
                extraZombiesPerRound = 1; break;
            case 10:
                extraZombiesPerRound = 3; break;
            case 15:
                extraZombiesPerRound = 4; break;
            case 25:
                extraZombiesPerRound = 5; break;
            case 32:
                extraZombiesPerRound = 6; break;
            case 36:
                extraZombiesPerRound = 7; break;
            case 39:
                extraZombiesPerRound = 8; break;
            case 48:
                extraZombiesPerRound = 9; break;
            case 54:
                extraZombiesPerRound = 10; break;
            case 60:
                extraZombiesPerRound = 12; break;
            case 70:
                extraZombiesPerRound = 13; break;
            case 77:
                extraZombiesPerRound = 14; break;
            case 83:
                extraZombiesPerRound = 15; break;
            case 90:
                extraZombiesPerRound = 16; break;
            case 95:
                extraZombiesPerRound = 17; break;
            case 100:
                extraZombiesPerRound = 18; break;
            case 105:
                extraZombiesPerRound = 19; break;
            case 110:
                extraZombiesPerRound = 20; break;
            case 130:
                extraZombiesPerRound = 30; break;
            default:
                break;
        }
        zombiesToSpawnThisRound = zombiesKilledThisRound + extraZombiesPerRound;
        zombiesKilledThisRound = 0;
        spawnedZombies = 0;
        zombiesToSpawnThisRound = totalZombiesToSpawn * currentRound;
        roundHasChanged = false;
    }

    private void Update()
    {
        if(!roundHasChanged)
        {
            if (zombiesKilledThisRound >= zombiesToSpawnThisRound)
            {
                currentRound++;
                hudManager.UpdateRoundText(currentRound);
                StartCoroutine(StartSpawning());
                roundHasChanged = true;
            }
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
                                print(spawnedZombies);
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
