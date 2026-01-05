using UnityEngine;

// Lưu ý: Tạo ScriptableObject này trong Assets/Resources/Settings/GameSettings.asset
[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Game Settings")]
public class GameSettings : ScriptableObject
{
    // Singleton instance
    private static GameSettings _instance;
    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameSettings>("Settings/GameSettings");

                if (_instance == null)
                {
                    Debug.LogWarning("GameSettings not found in Resources. Creating temporary instance.");
                    _instance = CreateInstance<GameSettings>();
                }
            }
            return _instance;
        }
    }

    [Header("Player Settings")]
    public int startingLives = 3;
    public int maxLives = 5;
    public float playerBulletSpeed = 10f;
    public float playerFireRate = 0.2f;

    [Header("Energy Settings")]
    public float energyGainPerHit = 5f;
    public float energyGainPerKill = 10f;
    public float energyRegenerationRate = 10f;

    [Header("Item Drop Settings")]
    [Range(0f, 1f)] public float healthDropChance = 0.2f;
    [Range(0f, 1f)] public float energyDropChance = 0.3f;
    [Range(0f, 1f)] public float lifeDropChance = 0.1f;

    [Header("Game Settings")]
    public float gameOverDelay = 2f;
    public float stageTransitionDelay = 1f;
    public float itemDropChance = 0.3f; // Tổng tỉ lệ drop item

    [Header("Enemy Settings")]
    public float enemyBulletSpeed = 5f;
    public float bossBulletSpeed = 4f;
}