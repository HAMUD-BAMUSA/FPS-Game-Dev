using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class ZombieSpawnController : MonoBehaviour
{
    //SINGLETON 
    public static ZombieSpawnController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    

    public int initialZombiesPerWave = 5;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f;
    public float waveCooldown = 10f;

    public float spawnRadius = 10f;   // radius around spawner

    public GameObject zombiePrefab;

    // wave tracking
    public int CurrentWave { get; private set; } = 0;

    private bool inCooldown = false;
    public float cooldownCounter = 0;

    public List<Enemy> currentZombiesAlive = new List<Enemy>();

    public TextMeshProUGUI titleWaveOverUI;
    public TextMeshProUGUI cooldownCounterTitleUI;
    public TextMeshProUGUI currentWaveUI;

    void Start()
    {
        // Initialize the number of zombies for the first wave
        currentZombiesPerWave = initialZombiesPerWave;
        StartNextWave();
    }

    void StartNextWave()
    {
        CurrentWave++; // increase wave no
        currentZombiesAlive.Clear();
        currentWaveUI.text = "Wave: " + CurrentWave;
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        // Spawn zombies one by one with a delay between each
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            Vector3 spawnPosition;

            if (TryGetNavMeshPosition(out spawnPosition))
            {
                GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
                Enemy enemy = zombie.GetComponent<Enemy>();
                currentZombiesAlive.Add(enemy);
            }
            else
            {
                Debug.LogWarning("Failed to find NavMesh position for zombie spawn.");
            }

            // Wait a bit before spawning the next zombie
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Tries to find a random position on the NavMesh near the spawner
    bool TryGetNavMeshPosition(out Vector3 result)
    {
        for (int i = 0; i < 10; i++) // try 10 times
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPoint.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    void Update()
    {
        // Remove dead or null zombies from alive list
        currentZombiesAlive.RemoveAll(z => z == null || z.isDead);

        // Start cooldown if all zombies are dead and not already in cooldown
        if (currentZombiesAlive.Count == 0 && !inCooldown)
        {
            StartCoroutine(WaveCooldown());
        }

        // Cooldown countdown
        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            // Reset counter when not in cooldown
            cooldownCounter = waveCooldown;
        }

        // Update UI
        if (cooldownCounterTitleUI != null)
            cooldownCounterTitleUI.text = cooldownCounter.ToString("F1");
    }

    IEnumerator WaveCooldown()
    {
        inCooldown = true;
        titleWaveOverUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;
        titleWaveOverUI.gameObject.SetActive(false);

        currentZombiesPerWave *= 2; // multi zs by 2 in nxt wave
        StartNextWave();
    }
}
