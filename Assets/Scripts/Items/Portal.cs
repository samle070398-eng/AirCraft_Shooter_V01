using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmplitude = 0.2f;
    [SerializeField] private GameObject enterEffect;
    [SerializeField] private AudioClip portalSound;

    private int nextStageIndex = 1;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;

        // Add trigger collider
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 1f;
        }
    }

    private void Update()
    {
        // Rotate portal
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Pulse effect
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        transform.localScale = originalScale * (1 + pulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EnterPortal(collision.gameObject);
        }
    }

    public void SetNextStage(int stageIndex)
    {
        nextStageIndex = stageIndex;
    }

    private void EnterPortal(GameObject player)
    {
        // Play effects
        if (enterEffect != null)
        {
            Instantiate(enterEffect, transform.position, Quaternion.identity);
        }

        // Play sound
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null && portalSound != null)
        {
            audioManager.PlaySFX(portalSound);
        }

        // Disable player during transition
        player.SetActive(false);

        // Load next stage
        StageManager stageManager = FindFirstObjectByType<StageManager>();
        if (stageManager != null)
        {
            stageManager.LoadNextStage();
        }
        else
        {
            // Fallback to SceneLoader
            SceneLoader.Instance.LoadScene($"Stage{nextStageIndex}");
        }

        // Destroy portal
        Destroy(gameObject);
    }
}