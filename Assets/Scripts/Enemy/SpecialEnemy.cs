using UnityEngine;

public class SpecialEnemy : EnemyBase
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private int bulletsPerShot = 3;
    [SerializeField] private float spreadAngle = 30f;

    private float fireTimer = 0f;
    private Vector3 lastPlayerPosition;

    protected override void Start()
    {
        base.Start();
        fireTimer = fireRate; // Shoot immediately
    }

    protected override void Move()
    {
        // Move left and right across screen
        if (transform.position.x > 7f || transform.position.x < -7f)
        {
            speed = -speed;
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Update shooting timer
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0 && playerTransform != null)
        {
            ShootAtPlayer();
            fireTimer = fireRate;
        }

        // Store player position for shooting
        if (playerTransform != null)
        {
            lastPlayerPosition = playerTransform.position;
        }
    }

    private void ShootAtPlayer()
    {
        if (enemyBulletPrefab == null) return;

        if (bulletsPerShot == 1)
        {
            // Single bullet aimed at player
            Vector3 direction = (lastPlayerPosition - transform.position).normalized;
            CreateBullet(direction);
        }
        else
        {
            // Spread shot
            Vector3 baseDirection = (lastPlayerPosition - transform.position).normalized;
            float angleStep = spreadAngle / (bulletsPerShot - 1);
            float startAngle = -spreadAngle / 2;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector3 direction = Quaternion.Euler(0, 0, angle) * baseDirection;
                CreateBullet(direction);
            }
        }
    }

    private void CreateBullet(Vector3 direction)
    {
        GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);

        // Rotate bullet to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Set bullet velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * 5f; // Adjust speed as needed
        }
    }
}