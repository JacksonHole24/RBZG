using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Types")]
    [Tooltip("The walker zombie type scriptable object.")]
    public List<Zombie> walkerZombies;
    [Tooltip("The jogger zombie type scriptable object.")]
    public List<Zombie> joggerZombie;
    [Tooltip("The runner zombie type scriptable object.")]
    public List<Zombie> runnerZombie;

    [Header("Spawn Stats")]
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

    [Header("Waves")]
    [Tooltip("All the wave changes scriptable objects")]
    public List<Wave> waveChanges;
    [Tooltip("Place the wave generator scriptable object")]
    public WaveGenerator waveGenerator;

    private int emptyCount = 0;
    private Collider[] colliders;
    private int spawnedZombies = 0;
    private float spawnTimer = 0f;
    private int currentRound = 1;
    private int zombiesToSpawnThisRound = 0;
    [HideInInspector] public int zombiesKilledThisRound = 0;
    private bool roundHasChanged = false;
    private int extraZombiesPerRound = 0;
    private float minJoggerZombies = 0f;
    private float maxJoggerZombies = 0f;
    private int joggerZomsToSpawn = 0;
    private int joggerZomsSpawned = 0;
    private float minRunnerZombies = 0f;
    private float maxRunnerZombies = 0f;
    private int runnerZomsToSpawn = 0;
    private int runnerZomsSpawned = 0;



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
        foreach (Wave x in waveChanges)
        {
            if(x.wave == currentRound)
            {
                extraZombiesPerRound = x.extraZombiesPerWave;

                minJoggerZombies = x.newMinRunnersPerWave;
                maxJoggerZombies = x.newMaxRunnersPerWave;

                minRunnerZombies = x.newMinRunnersPerWave;
                maxRunnerZombies = x.newMaxRunnersPerWave;
            }
        }

        zombiesToSpawnThisRound = zombiesKilledThisRound + extraZombiesPerRound;
        zombiesKilledThisRound = 0;
        spawnedZombies = 0;
        zombiesToSpawnThisRound = totalZombiesToSpawn * currentRound;
        roundHasChanged = false;

        runnerZomsToSpawn = GetZombieChance(minRunnerZombies, maxRunnerZombies);

        joggerZomsToSpawn = GetZombieChance(minJoggerZombies, maxJoggerZombies);
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
                                if(joggerZomsToSpawn > 0 && joggerZomsSpawned < joggerZomsToSpawn)
                                {
                                    SpawnZombie(collider, joggerZombie);
                                    joggerZomsSpawned++;
                                }
                                if (runnerZomsToSpawn > 0 && runnerZomsSpawned < runnerZomsToSpawn)
                                {
                                    SpawnZombie(collider, runnerZombie);
                                    runnerZomsSpawned++;
                                }
                                else
                                {
                                    SpawnZombie(collider, walkerZombies);
                                }
                                
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

    private void SpawnZombie(Collider spawnCollider, List<Zombie> zomList)
    {
        int ran = Random.Range(0, zomList.Count);
        GameObject instantiated = Instantiate(zomList[ran].prefab, spawnCollider.transform.position, Quaternion.identity);
        instantiated.GetComponent<ZombieAI>().zombieStats = zomList[ran];
        spawnedZombies++;
        print(spawnedZombies);
        print(instantiated.GetComponent<ZombieAI>().zombieStats.zombieType);
    }

    private int GetZombieChance(float min, float max)
    {
        float chance = Random.Range(min, max);
        if (chance > 0)
        {
            float zomsToSpawnFloat = 0;
            zomsToSpawnFloat = zombiesToSpawnThisRound * chance;
            return (int)zomsToSpawnFloat;
        }
        else return 0;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the spawn radius.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
