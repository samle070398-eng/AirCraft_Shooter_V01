using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;

    [Header("Energy")]
    [SerializeField] private Slider energySlider;
    [SerializeField] private Text energyText;

    [Header("Lives")]
    [SerializeField] private Text livesText;

    [Header("Score")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;

    [Header("Wave/Stage")]
    [SerializeField] private Text waveText;
    [SerializeField] private Text stageText;

    [Header("Boss Health")]
    [SerializeField] private GameObject bossHealthPanel;
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private Text bossNameText;
    [SerializeField] private Text bossHealthText;

    private HealthSystem currentBossHealth;

    private void Start()
    {
        // Subscribe to events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
        }

        // Find player and subscribe to their events
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            EnergySystem energySystem = player.GetComponent<EnergySystem>();
            LivesSystem livesSystem = player.GetComponent<LivesSystem>();

            if (healthSystem != null)
            {
                healthSystem.OnHealthChanged.AddListener(UpdateHealthUI);
            }

            if (energySystem != null)
            {
                energySystem.OnEnergyChanged.AddListener(UpdateEnergyUI);
            }

            if (livesSystem != null)
            {
                livesSystem.OnLivesChanged.AddListener(UpdateLivesUI);
            }
        }

        // Initialize UI
        UpdateScore(GameManager.Instance?.currentScore ?? 0);
        UpdateHighScore();

        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(false);
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }

    private void UpdateEnergyUI(float currentEnergy, float maxEnergy)
    {
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }

        if (energyText != null)
        {
            energyText.text = $"{(int)currentEnergy}/{(int)maxEnergy}";
        }
    }

    private void UpdateLivesUI(int lives)
    {
        if (livesText != null)
        {
            livesText.text = $"LIVES: {lives}";
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {score}";
        }
    }

    private void UpdateHighScore()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"HIGH SCORE: {GameManager.Instance?.highScore ?? 0}";
        }
    }

    public void ShowBossHealth(int currentHealth, int maxHealth, string bossName = "BOSS")
    {
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(true);

            if (bossHealthSlider != null)
            {
                bossHealthSlider.maxValue = maxHealth;
                bossHealthSlider.value = currentHealth;
            }

            if (bossNameText != null)
            {
                bossNameText.text = bossName;
            }

            if (bossHealthText != null)
            {
                bossHealthText.text = $"{currentHealth}/{maxHealth}";
            }
        }
    }
    public void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        if (bossHealthPanel != null && bossHealthPanel.activeSelf)
        {
            if (bossHealthSlider != null)
            {
                bossHealthSlider.value = currentHealth;
            }

            if (bossHealthText != null)
            {
                bossHealthText.text = $"{currentHealth}/{maxHealth}";
            }
        }
    }

    public void HideBossHealth()
    {
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(false);
        }
    }

    public void UpdateStageText(int stage)
    {
        if (stageText != null)
        {
            stageText.text = $"STAGE {stage}";
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
        }
    }
}