using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private GameObject hitEffect;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Auto-destroy after lifetime
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector2 direction, int bulletDamage = 10)
    {
        damage = bulletDamage;

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }

        // Rotate bullet to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem playerHealth = collision.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                CreateHitEffect();
                Destroy(gameObject);
            }
        }
        else if (collision.CompareTag("Boundary"))
        {
            CreateHitEffect();
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