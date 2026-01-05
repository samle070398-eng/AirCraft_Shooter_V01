using UnityEngine;

public class LifeItem : MonoBehaviour
{
    [Header("Life Settings")]
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
        LivesSystem livesSystem = player.GetComponent<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.GainLife();

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