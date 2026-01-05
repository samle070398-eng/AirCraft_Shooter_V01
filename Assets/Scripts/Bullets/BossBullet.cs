using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private bool homing = false;
    [SerializeField] private float homingStrength = 2f;

    private Rigidbody2D rb;
    private Transform target;
    private Vector3 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Auto-destroy after lifetime
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector3 shootDirection, int bulletDamage = 20)
    {
        direction = shootDirection.normalized;
        damage = bulletDamage;

        // Find player for homing
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && homing)
        {
            target = player.transform;
        }

        // Initial velocity
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }

        // Rotate bullet to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        if (homing && target != null)
        {
            // Homing towards player
            Vector3 targetDirection = (target.position - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDirection, homingStrength * Time.deltaTime);

            // Update velocity and rotation
            rb.linearVelocity = direction * speed;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
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
        else if (collision.CompareTag("Boundary") || collision.CompareTag("PlayerBullet"))
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