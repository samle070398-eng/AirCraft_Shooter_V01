using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public string waveName;
    public int waveNumber;
    public List<EnemyWave> enemyWaves;
    public float timeBeforeNextWave = 3f;
    public bool isBossWave = false;
    public GameObject bossPrefab;
}

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval = 0.5f;
    public Transform spawnPoint;
}

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private Transform[] spawnPoints;

    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Text waveText;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private int enemiesRemaining = 0;

    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveStarted;
    public event WaveEvent OnWaveCompleted;

    private void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            // All waves completed
            Debug.Log("All waves completed!");
            return;
        }

        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        // Update UI
        if (waveText != null)
        {
            waveText.text = $"WAVE {wave.waveNumber}: {wave.waveName}";
        }

        // Notify wave start
        OnWaveStarted?.Invoke(wave.waveNumber);

        enemiesRemaining = 0;

        // Spawn enemies
        foreach (EnemyWave enemyWave in wave.enemyWaves)
        {
            enemiesRemaining += enemyWave.count;

            for (int i = 0; i < enemyWave.count; i++)
            {
                Transform spawnPoint = enemyWave.spawnPoint != null ?
                    enemyWave.spawnPoint : GetRandomSpawnPoint();

                if (enemyWave.enemyPrefab != null)
                {
                    Instantiate(enemyWave.enemyPrefab, spawnPoint.position, Quaternion.identity);
                }

                yield return new WaitForSeconds(enemyWave.spawnInterval);
            }
        }

        // Spawn boss if this is a boss wave
        if (wave.isBossWave && wave.bossPrefab != null)
        {
            enemiesRemaining++;
            Transform bossSpawnPoint = GetRandomSpawnPoint();
            GameObject boss = Instantiate(wave.bossPrefab, bossSpawnPoint.position, Quaternion.identity);

            // Setup boss health bar
            HealthSystem bossHealth = boss.GetComponent<HealthSystem>();
            if (bossHealth != null)
            {
                bossHealth.OnDeath.AddListener(OnBossDefeated);
            }
        }

        isSpawning = false;

        // Wait for all enemies to be defeated
        yield return new WaitUntil(() => enemiesRemaining <= 0);

        // Wave completed
        OnWaveCompleted?.Invoke(wave.waveNumber);

        // Wait before next wave
        yield return new WaitForSeconds(wave.timeBeforeNextWave);

        currentWaveIndex++;
        StartNextWave();
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0) return transform;
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    public void OnEnemyDestroyed()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0 && !isSpawning)
        {
            // Current wave is cleared
            OnWaveCompleted?.Invoke(waves[currentWaveIndex].waveNumber);
        }
    }

    private void OnBossDefeated()
    {
        enemiesRemaining--;

        // Spawn portal to next stage
        SpawnPortal();
    }

    private void SpawnPortal()
    {
        // Create portal prefab at center of screen
        GameObject portalPrefab = Resources.Load<GameObject>("Prefabs/Portal");
        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}