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

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;

    private int currentWave = 1;
    private int currentWaveSize;
    private readonly List<EnemyHealth> aliveEnemies = new List<EnemyHealth>();
    private Coroutine waveRoutine;

    private void Start()
    {
        currentWaveSize = Mathf.Max(1, startWaveSize);
        UpdateWaveText();
        waveRoutine = StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentWaveSize; i++)
        {
            SpawnSingleEnemy();
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
        }
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

        EnemyAI ai = instance.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.player = player;
            ai.waypoints = null;
        }

        EnemyHealth health = instance.GetComponent<EnemyHealth>();
        if (health != null)
        {
            aliveEnemies.Add(health);
            health.OnDeath += HandleEnemyDeath;
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

    private void HandleEnemyDeath(EnemyHealth health)
    {
        if (health == null)
        {
            return;
        }

        health.OnDeath -= HandleEnemyDeath;
        aliveEnemies.Remove(health);

        if (aliveEnemies.Count == 0)
        {
            AdvanceWave();
        }
    }

    private void AdvanceWave()
    {
        currentWave++;
        currentWaveSize++;
        UpdateWaveText();

        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
        }
        waveRoutine = StartCoroutine(SpawnWave());
    }

    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = $"Oleada: {currentWave}";
        }
    }
}
