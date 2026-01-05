using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector2 moveBounds = new Vector2(7f, 4f);

    [Header("Shooting")]
    [SerializeField] private GameObject bossBulletPrefab;
    [SerializeField] private Transform[] bulletSpawnPoints;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int bulletDamage = 20;

    [Header("Minion Spawning")]
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private Transform[] minionSpawnPoints;
    [SerializeField] private float minionSpawnRate = 3f;
    [SerializeField] private int minionsPerWave = 3;

    [Header("Patterns")]
    [SerializeField] private float patternDuration = 10f;
    [SerializeField] private bool useCircularPattern = true;

    private HealthSystem healthSystem;
    private float fireTimer = 0f;
    private float minionTimer = 0f;
    private float patternTimer = 0f;
    private bool movingRight = true;
    private Vector3 targetPosition;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            healthSystem = gameObject.AddComponent<HealthSystem>();
        }

        healthSystem.Initialize(maxHealth);
        healthSystem.OnHealthChanged.AddListener(OnHealthChanged);
        healthSystem.OnDeath.AddListener(OnDeath);

        // Set random initial position
        targetPosition = new Vector3(
            Random.Range(-moveBounds.x, moveBounds.x),
            moveBounds.y,
            0
        );

        // Start pattern coroutine
        StartCoroutine(BossPattern());
    }

    private void Update()
    {
        Move();
        UpdateTimers();

        if (fireTimer <= 0)
        {
            Shoot();
            fireTimer = fireRate;
        }

        if (minionTimer <= 0)
        {
            SpawnMinions();
            minionTimer = minionSpawnRate;
        }
    }

    private void Move()
    {
        // Smooth movement to target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Update pattern timer
        patternTimer += Time.deltaTime;
        if (patternTimer >= patternDuration)
        {
            patternTimer = 0;
            movingRight = !movingRight;
        }
    }

    private IEnumerator BossPattern()
    {
        while (true)
        {
            // Pattern 1: Move left to right
            for (int i = 0; i < 5; i++)
            {
                targetPosition = new Vector3(
                    movingRight ? moveBounds.x : -moveBounds.x,
                    moveBounds.y - (i * 1.5f),
                    0
                );
                yield return new WaitForSeconds(1f);
            }

            // Pattern 2: Circular movement
            if (useCircularPattern)
            {
                float radius = 3f;
                float angle = 0f;
                for (int i = 0; i < 20; i++)
                {
                    angle += 18f;
                    float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                    float y = moveBounds.y - 2f + Mathf.Sin(angle * Mathf.Deg2Rad) * 1f;
                    targetPosition = new Vector3(x, y, 0);
                    yield return new WaitForSeconds(0.2f);
                }
            }

            movingRight = !movingRight;
        }
    }

    private void Shoot()
    {
        if (bossBulletPrefab == null || bulletSpawnPoints.Length == 0) return;

        // Find player for aiming
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 targetDirection = Vector3.down; // Default shoot down

        if (player != null)
        {
            // Aim at player
            targetDirection = (player.transform.position - transform.position).normalized;
        }

        // Shoot from all spawn points
        foreach (Transform spawnPoint in bulletSpawnPoints)
        {
            GameObject bullet = Instantiate(bossBulletPrefab, spawnPoint.position, Quaternion.identity);

            // Set bullet properties
            BossBullet bossBullet = bullet.GetComponent<BossBullet>();
            if (bossBullet != null)
            {
                bossBullet.Initialize(targetDirection, bulletDamage);
            }
        }
    }
    private void UpdateTimers()
    {
        fireTimer -= Time.deltaTime;
        minionTimer -= Time.deltaTime;
    }

    private void SpawnMinions()
    {
        if (minionPrefab == null || minionSpawnPoints.Length == 0) return;

        for (int i = 0; i < Mathf.Min(minionsPerWave, minionSpawnPoints.Length); i++)
        {
            Transform spawnPoint = minionSpawnPoints[Random.Range(0, minionSpawnPoints.Length)];
            Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnHealthChanged(int current, int max)
    {
        // Update UI
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowBossHealth(current, max);
        }

        // Increase aggression at low health
        if (current < max * 0.3f)
        {
            fireRate *= 0.7f;
            minionSpawnRate *= 0.7f;
        }
    }

    private void OnDeath()
    {
        // Spawn portal to next stage
        SpawnPortal();

        // Notify game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Victory();
        }

        Destroy(gameObject);
    }

    private void SpawnPortal()
    {
        GameObject portalPrefab = Resources.Load<GameObject>("Prefabs/Portal");
        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}