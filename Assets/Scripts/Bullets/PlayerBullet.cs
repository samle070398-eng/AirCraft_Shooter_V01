using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private int damage = 15;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private bool piercing = false;
    [SerializeField] private int maxPierceCount = 0;

    private Rigidbody2D rb;
    private int piercedCount = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Auto-destroy after lifetime
        Destroy(gameObject, lifeTime);
    }

    private void Start()
    {
        // Player bullets always shoot up
        if (rb != null)
        {
            rb.linearVelocity = Vector2.up * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (!piercing || piercedCount >= maxPierceCount)
                {
                    CreateHitEffect();
                    Destroy(gameObject);
                }
                else
                {
                    piercedCount++;
                }
            }
        }
        else if (collision.CompareTag("Boss"))
        {
            HealthSystem bossHealth = collision.GetComponent<HealthSystem>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
                CreateHitEffect();
                Destroy(gameObject);
            }
        }
        else if (collision.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }
    }

    private void CreateHitEffect()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
    }
}