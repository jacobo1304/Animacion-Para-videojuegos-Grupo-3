using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs (must have EnemyAI)")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private int startWaveSize = 2;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 4f;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Waypoints")]
    [SerializeField] private Transform[] waypoints;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Power Ups")]
    [SerializeField] private float powerUpSpawnChance = 1f;
    [SerializeField] private Transform[] powerUpSpawnPoints;
    [SerializeField] private List<PowerUpSpawnEntry> powerUpPrefabs = new List<PowerUpSpawnEntry>();

    private int currentWave = 1;
    private int currentWaveSize;
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();
    private Coroutine waveRoutine;
    private int spawnedThisWave;
    private bool waveSpawning;
    private readonly Dictionary<PowerUpType, GameObject> activePowerUps = new Dictionary<PowerUpType, GameObject>();

    [System.Serializable]
    private class PowerUpSpawnEntry
    {
        public PowerUpType type;
        public GameObject prefab;
    }

    private void Start()
    {
        currentWaveSize = Mathf.Max(1, startWaveSize);
        UpdateWaveText();
        TrySpawnPowerUp();
        spawnedThisWave = 0;
        waveSpawning = true;
        waveRoutine = StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        CleanupInactiveEnemies();
        TryAdvanceWaveIfReady();
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentWaveSize; i++)
        {
            SpawnSingleEnemy();
            spawnedThisWave++;
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
        }

        waveSpawning = false;
        TryAdvanceWaveIfReady();
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("WaveSpawner: No enemy prefabs assigned.", this);
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        if (prefab == null)
        {
            Debug.LogWarning("WaveSpawner: Null enemy prefab in list.", this);
            return;
        }

        if (prefab.GetComponent<EnemyAI>() == null)
        {
            Debug.LogWarning($"WaveSpawner: Prefab '{prefab.name}' has no EnemyAI, skipping.", this);
            return;
        }

        Transform spawnPoint = GetSpawnPoint();
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject instance = Instantiate(prefab, position, rotation);
        spawnedEnemies.Add(instance);

        EnemyAI ai = instance.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.player = player;
            if (waypoints != null && waypoints.Length > 0)
            {
                ai.waypoints = waypoints;
            }
        }
    }

    private Transform GetSpawnPoint()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        return null;
    }

    private void CleanupInactiveEnemies()
    {
        if (spawnedEnemies.Count == 0)
        {
            return;
        }

        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = spawnedEnemies[i];
            if (enemy == null || !enemy.activeInHierarchy)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
                spawnedEnemies.RemoveAt(i);
            }
        }
    }

    private void TryAdvanceWaveIfReady()
    {
        if (waveSpawning)
        {
            return;
        }

        if (spawnedThisWave < currentWaveSize)
        {
            return;
        }

        if (spawnedEnemies.Count > 0)
        {
            return;
        }

        AdvanceWave();
    }

    private void AdvanceWave()
    {
        currentWave++;
        currentWaveSize++;
        UpdateWaveText();
        TrySpawnPowerUp();

        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
        }
        spawnedThisWave = 0;
        waveSpawning = true;
        waveRoutine = StartCoroutine(SpawnWave());
    }

    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = $"Oleada: {currentWave}";
        }
    }

    private void TrySpawnPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Count == 0)
        {
            return;
        }

        if (Random.value > Mathf.Clamp01(powerUpSpawnChance))
        {
            return;
        }

        CleanupDestroyedPowerUps();

        List<PowerUpSpawnEntry> available = new List<PowerUpSpawnEntry>();
        foreach (var entry in powerUpPrefabs)
        {
            if (entry == null || entry.prefab == null)
            {
                continue;
            }

            if (!activePowerUps.ContainsKey(entry.type) || activePowerUps[entry.type] == null)
            {
                available.Add(entry);
            }
        }

        if (available.Count == 0)
        {
            return;
        }

        PowerUpSpawnEntry chosen = available[Random.Range(0, available.Count)];
        Transform spawnPoint = GetPowerUpSpawnPoint();
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject instance = Instantiate(chosen.prefab, position, rotation);
        activePowerUps[chosen.type] = instance;
    }

    private Transform GetPowerUpSpawnPoint()
    {
        if (powerUpSpawnPoints != null && powerUpSpawnPoints.Length > 0)
        {
            return powerUpSpawnPoints[Random.Range(0, powerUpSpawnPoints.Length)];
        }

        return GetSpawnPoint();
    }

    private void CleanupDestroyedPowerUps()
    {
        if (activePowerUps.Count == 0)
        {
            return;
        }

        var keys = new List<PowerUpType>(activePowerUps.Keys);
        foreach (var key in keys)
        {
            if (activePowerUps[key] == null)
            {
                activePowerUps.Remove(key);
            }
        }
    }
}
