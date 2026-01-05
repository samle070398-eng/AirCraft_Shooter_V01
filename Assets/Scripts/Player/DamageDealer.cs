using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 10;
    public string targetTag = "Enemy"; // hoặc "Player"

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            HealthSystem healthSystem = collision.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
            }

            // Destroy bullet
            if (!gameObject.CompareTag("Player") && !gameObject.CompareTag("Enemy"))
            {
                Destroy(gameObject);
            }
        }
    }
}