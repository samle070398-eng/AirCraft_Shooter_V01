using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float maxLaserLength = 20f;
    [SerializeField] private float damagePerSecond = 50f;
    [SerializeField] private LayerMask laserTargetMask;
    [SerializeField] private GameObject laserHitEffect;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem laserStartEffect;
    [SerializeField] private ParticleSystem laserEndEffect;

    private bool isActive = false;
    private float activeTimer = 0f;
    private EnergySystem energySystem;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        energySystem = GetComponent<EnergySystem>();

        DeactivateLaser();
    }

    private void Update()
    {
        if (isActive)
        {
            UpdateLaser();

            // Consume energy per second
            if (energySystem != null && !energySystem.UseEnergy(10f * Time.deltaTime))
            {
                DeactivateLaser();
            }
        }
    }

    public void ActivateLaser()
    {
        if (isActive || (energySystem != null && !energySystem.HasEnoughEnergy(5f)))
            return;

        isActive = true;
        activeTimer = 0f;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }

        if (laserStartEffect != null)
        {
            laserStartEffect.Play();
        }
    }

    public void DeactivateLaser()
    {
        isActive = false;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        if (laserStartEffect != null)
        {
            laserStartEffect.Stop();
        }

        if (laserEndEffect != null)
        {
            laserEndEffect.Stop();
        }
    }

    private void UpdateLaser()
    {
        activeTimer += Time.deltaTime;

        // Calculate laser direction (always up from player)
        Vector3 laserStart = transform.position;
        Vector3 laserDirection = Vector3.up;
        Vector3 laserEnd = laserStart + (laserDirection * maxLaserLength);

        // Raycast to find hit point
        RaycastHit2D hit = Physics2D.Raycast(laserStart, laserDirection, maxLaserLength, laserTargetMask);

        if (hit.collider != null)
        {
            laserEnd = hit.point;

            // Damage enemy or boss
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)(damagePerSecond * Time.deltaTime));
                    CreateHitEffect(hit.point);
                }
            }
            else if (hit.collider.CompareTag("Boss"))
            {
                HealthSystem bossHealth = hit.collider.GetComponent<HealthSystem>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage((int)(damagePerSecond * Time.deltaTime));
                    CreateHitEffect(hit.point);
                }
            }
        }

        // Update line renderer
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, laserStart);
            lineRenderer.SetPosition(1, laserEnd);

            // Animate laser width
            float width = Mathf.Lerp(0.1f, 0.3f, Mathf.PingPong(activeTimer * 10f, 1f));
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width * 0.5f;
        }

        // Update end effect
        if (laserEndEffect != null)
        {
            laserEndEffect.transform.position = laserEnd;
            if (!laserEndEffect.isPlaying)
            {
                laserEndEffect.Play();
            }
        }
    }

    private void CreateHitEffect(Vector3 position)
    {
        if (laserHitEffect != null)
        {
            GameObject effect = Instantiate(laserHitEffect, position, Quaternion.identity);
            Destroy(effect, 0.5f);
        }
    }

    public bool IsActive() => isActive;
}