using UnityEngine;

public class BombEnemy : EnemyBase
{
    [Header("Sine Wave Settings")]
    [SerializeField] private float sineAmplitude = 3f;
    [SerializeField] private float sineFrequency = 1.5f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 30;

    private float sineTimer = 0f;

    protected override void Move()
    {
        // Move down with sine wave pattern
        float verticalMovement = -speed * Time.deltaTime;

        // Calculate sine wave movement
        sineTimer += Time.deltaTime;
        float horizontalMovement = Mathf.Sin(sineTimer * sineFrequency) * sineAmplitude * Time.deltaTime;

        // Apply movement
        transform.Translate(new Vector3(horizontalMovement, verticalMovement, 0));

        // Destroy if out of bounds
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    protected override void Die()
    {
        // Create explosion
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Damage nearby enemies and player
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    HealthSystem playerHealth = collider.GetComponent<HealthSystem>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(explosionDamage);
                    }
                }
                else if (collider.CompareTag("Enemy") && collider.gameObject != gameObject)
                {
                    EnemyBase enemy = collider.GetComponent<EnemyBase>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(explosionDamage);
                    }
                }
            }

            // Destroy explosion after animation
            Destroy(explosion, 1f);
        }

        base.Die();
    }
}