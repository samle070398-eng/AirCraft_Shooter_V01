using UnityEngine;

public class EnergyItem : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private float energyAmount = 30f;
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
        EnergySystem energySystem = player.GetComponent<EnergySystem>();
        if (energySystem != null)
        {
            energySystem.AddEnergy(energyAmount);

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