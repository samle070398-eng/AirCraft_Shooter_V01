using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int healAmount = 25;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect(collision.gameObject);
        }
    }

    private void Collect(GameObject player)
    {
        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.Heal(healAmount);

            // Play effects
            PlayCollectEffects();

            Destroy(gameObject);
        }
    }

    private void PlayCollectEffects()
    {
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Play sound if AudioManager exists
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null && collectSound != null)
        {
            audioManager.PlaySFX(collectSound);
        }
    }
}