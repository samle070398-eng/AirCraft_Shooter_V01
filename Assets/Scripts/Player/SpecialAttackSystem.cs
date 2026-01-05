using UnityEngine;

public class SpecialAttackSystem : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private float laserDamagePerSecond = 50f;
    [SerializeField] private float laserDuration = 2f;
    [SerializeField] private float laserCooldown = 5f;
    [SerializeField] private LayerMask laserTargetMask;

    [Header("Energy Cost")]
    [SerializeField] private float laserEnergyCost = 50f;

    private EnergySystem energySystem;
    private bool isLaserActive = false;
    private float laserTimer = 0f;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        energySystem = GetComponent<EnergySystem>();

        if (laserRenderer != null)
        {
            laserRenderer.enabled = false;
        }
    }

    private void Update()
    {
        UpdateTimers();

        if (isLaserActive)
        {
            UpdateLaser();
        }
    }

    public void ActivateLaser()
    {
        if (isLaserActive || cooldownTimer > 0 || !energySystem.UseEnergy(laserEnergyCost))
            return;

        isLaserActive = true;
        laserTimer = laserDuration;

        if (laserRenderer != null)
        {
            laserRenderer.enabled = true;
        }
    }

    private void UpdateLaser()
    {
        // Update laser position and rotation
        Vector3 laserStart = transform.position;
        Vector3 laserEnd = laserStart + Vector3.up * 20f; // Shoot upwards

        laserRenderer.SetPosition(0, laserStart);
        laserRenderer.SetPosition(1, laserEnd);

        // Damage enemies in laser path
        RaycastHit2D[] hits = Physics2D.RaycastAll(laserStart, Vector2.up, 20f, laserTargetMask);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                HealthSystem enemyHealth = hit.collider.GetComponent<HealthSystem>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((int)(laserDamagePerSecond * Time.deltaTime));
                }
            }
        }

        // Update timer
        laserTimer -= Time.deltaTime;
        if (laserTimer <= 0)
        {
            DeactivateLaser();
        }
    }

    private void DeactivateLaser()
    {
        isLaserActive = false;
        cooldownTimer = laserCooldown;

        if (laserRenderer != null)
        {
            laserRenderer.enabled = false;
        }
    }

    private void UpdateTimers()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public bool IsLaserReady() => !isLaserActive && cooldownTimer <= 0;
}