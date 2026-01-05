using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentScore = 0;
    public int highScore = 0;
    public int currentStage = 1;
    public int currentWave = 1;
    public bool isGamePaused = false;
    public bool isGameOver = false;
    public bool isVictory = false;

    [Header("Player Data")]
    public ShipData selectedShipData;
    public int playerLives;
    public int playerHealth;
    public int playerEnergy;

    [Header("UI References")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject victoryMenu;

    // Property để truy cập GameSettings
    public GameSettings Settings { get; private set; }

    // Events
    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

    public delegate void GameStateChanged(bool isPaused);
    public event GameStateChanged OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load GameSettings
            Settings = GameSettings.Instance;

            // Load high score
            highScore = PlayerPrefs.GetInt("HighScore", 0);

            // Set default lives from settings
            if (Settings != null)
            {
                playerLives = Settings.startingLives;
            }
            else
            {
                playerLives = 3;
            }

            // Scene load callback
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Stage"))
        {
            SetupStage();
        }
    }

    private void SetupStage()
    {
        isGameOver = false;
        isVictory = false;

        // Find and setup UI
        FindUIReferences();

        // Spawn player if not exists
        SpawnPlayer();
    }

    private void FindUIReferences()
    {
        // Find UI elements in scene
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        gameOverMenu = GameObject.FindGameObjectWithTag("GameOverMenu");
        victoryMenu = GameObject.FindGameObjectWithTag("VictoryMenu");

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
        if (victoryMenu != null) victoryMenu.SetActive(false);
    }

    private void SpawnPlayer()
    {
        if (selectedShipData == null || selectedShipData.shipPrefab == null)
        {
            Debug.LogError("No ship data selected!");
            return;
        }

        GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        Vector3 spawnPosition = playerSpawnPoint != null ?
            playerSpawnPoint.transform.position : Vector3.zero;

        GameObject player = Instantiate(selectedShipData.shipPrefab, spawnPosition, Quaternion.identity);

        // Initialize player systems with saved data
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Initialize(selectedShipData);
        }

        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            if (playerHealth > 0)
                healthSystem.Initialize(playerHealth);
            else
                healthSystem.Initialize(selectedShipData.maxHealth);
        }

        EnergySystem energySystem = player.GetComponent<EnergySystem>();
        if (energySystem != null)
        {
            if (playerEnergy > 0)
                energySystem.Initialize(playerEnergy);
            else
                energySystem.Initialize(selectedShipData.maxEnergy);
        }

        LivesSystem livesSystem = player.GetComponent<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.Initialize(playerLives > 0 ? playerLives : Settings.startingLives);
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    public void OnEnemyHit(EnemyBase enemy, int damage)
    {
        if (Settings == null) return;

        // Add energy for hitting enemy
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            EnergySystem energySystem = player.GetComponent<EnergySystem>();
            if (energySystem != null)
            {
                energySystem.AddEnergy(Settings.energyGainPerHit);
            }
        }
    }

    public void OnEnemyKilled(EnemyBase enemy)
    {
        if (Settings == null) return;

        // Add energy for killing enemy
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            EnergySystem energySystem = player.GetComponent<EnergySystem>();
            if (energySystem != null)
            {
                energySystem.AddEnergy(Settings.energyGainPerKill);
            }
        }
    }

    public void TogglePause()
    {
        if (isGameOver || isVictory) return;

        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }

        OnGameStateChanged?.Invoke(isGamePaused);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isGamePaused = true;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
        }
    }

    public void Victory()
    {
        isVictory = true;
        Time.timeScale = 0f;

        if (victoryMenu != null)
        {
            victoryMenu.SetActive(true);
        }
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SavePlayerData();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextStage()
    {
        currentStage++;
        SavePlayerData();
        SceneManager.LoadScene($"Stage{currentStage}");
    }

    public void SavePlayerData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            EnergySystem energySystem = player.GetComponent<EnergySystem>();
            LivesSystem livesSystem = player.GetComponent<LivesSystem>();

            if (healthSystem != null) playerHealth = healthSystem.GetCurrentHealth();
            if (energySystem != null) playerEnergy = (int)energySystem.GetCurrentEnergy();
            if (livesSystem != null) playerLives = livesSystem.GetCurrentLives();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}