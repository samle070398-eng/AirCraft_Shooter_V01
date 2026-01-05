using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] protected int health = 50;
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected int scoreValue = 100;
    [SerializeField] protected float itemDropChance = 0.2f;

    [Header("References")]
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] protected GameObject[] dropItems;

    protected Transform playerTransform;
    protected Vector2 movementDirection;

    protected virtual void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    protected virtual void Update()
    {
        Move();
    }

    protected abstract void Move();

    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        // Notify GameManager for score/energy gain
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyHit(this, damageAmount);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Add score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.OnEnemyKilled(this);
        }

        // Drop item
        TryDropItem();

        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    protected virtual void TryDropItem()
    {
        if (dropItems.Length == 0) return;

        float dropChance = itemDropChance;

        // SỬA Ở ĐÂY: GameManager.Instance.Settings thay vì GameManager.Instance.gameSettings
        if (dropChance < 0 && GameManager.Instance != null && GameManager.Instance.Settings != null)
        {
            dropChance = GameManager.Instance.Settings.itemDropChance;
        }

        if (Random.value <= dropChance)
        {
            GameObject itemToDrop = dropItems[Random.Range(0, dropItems.Length)];
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem playerHealth = collision.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Die();
        }
    }
}