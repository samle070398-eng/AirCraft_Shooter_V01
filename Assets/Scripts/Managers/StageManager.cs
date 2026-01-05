using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Stage Settings")]
    [SerializeField] private List<StageData> stages = new List<StageData>();
    [SerializeField] private int currentStageIndex = 0;

    [Header("Portal Settings")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private float portalSpawnDelay = 2f;

    [System.Serializable]
    public class StageData
    {
        public string stageName;
        public string sceneName;
        public int stageNumber;
        public bool isBossStage;
        public GameObject bossPrefab;
        public List<WaveData> waves;
    }

    [System.Serializable]
    public class WaveData
    {
        public string waveName;
        public List<EnemySpawnData> enemies;
        public float timeBeforeNextWave;
    }

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Stage"))
        {
            SetupCurrentStage();
        }
    }

    private void SetupCurrentStage()
    {
        if (currentStageIndex >= stages.Count)
        {
            Debug.Log("All stages completed!");
            return;
        }

        StageData currentStage = stages[currentStageIndex];

        // Setup wave manager
        WaveManager waveManager = FindAnyObjectByType<WaveManager>();
        if (waveManager != null && currentStage.waves.Count > 0)
        {
            // Convert StageData waves to WaveManager waves
            List<Wave> waves = new List<Wave>();

            foreach (WaveData waveData in currentStage.waves)
            {
                Wave wave = new Wave
                {
                    waveName = waveData.waveName,
                    waveNumber = waves.Count + 1,
                    timeBeforeNextWave = waveData.timeBeforeNextWave,
                    isBossWave = currentStage.isBossStage && (waves.Count == currentStage.waves.Count - 1),
                    bossPrefab = currentStage.isBossStage ? currentStage.bossPrefab : null
                };

                waves.Add(wave);
            }

            // Set waves in WaveManager
            // Note: You'll need to make waves public in WaveManager or create a method to set them
        }

        // Update UI
        UIManager uiManager = FindAnyObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateStageText(currentStage.stageNumber);
        }
    }

    public void CompleteCurrentStage()
    {
        // Save player data
        GameManager.Instance?.SavePlayerData();

        // Check if this is the last stage
        if (currentStageIndex >= stages.Count - 1)
        {
            // Final stage completed
            GameManager.Instance?.Victory();
        }
        else
        {
            // Spawn portal to next stage
            SpawnPortal();
        }
    }

    private void SpawnPortal()
    {
        if (portalPrefab == null) return;

        StartCoroutine(SpawnPortalDelayed());
    }

    private System.Collections.IEnumerator SpawnPortalDelayed()
    {
        yield return new WaitForSeconds(portalSpawnDelay);

        // Spawn portal at center of screen
        Vector3 portalPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        GameObject portal = Instantiate(portalPrefab, portalPosition, Quaternion.identity);

        // Add portal behavior
        Portal portalScript = portal.GetComponent<Portal>();
        if (portalScript == null)
        {
            portalScript = portal.AddComponent<Portal>();
        }

        portalScript.SetNextStage(currentStageIndex + 1);
    }

    public void LoadStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= stages.Count)
        {
            Debug.LogError($"Invalid stage index: {stageIndex}");
            return;
        }

        currentStageIndex = stageIndex;
        SceneLoader.Instance.LoadScene(stages[stageIndex].sceneName); // Sửa thành Instance
    }

    public void LoadNextStage()
    {
        currentStageIndex++;
        if (currentStageIndex < stages.Count)
        {
            LoadStage(currentStageIndex);
        }
        else
        {
            Debug.Log("All stages completed!");
            GameManager.Instance?.Victory();
        }
    }

    public int GetCurrentStageNumber() => currentStageIndex + 1;
    public int GetTotalStages() => stages.Count;
    public string GetCurrentStageName() =>
        currentStageIndex < stages.Count ? stages[currentStageIndex].stageName : "Unknown";

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}