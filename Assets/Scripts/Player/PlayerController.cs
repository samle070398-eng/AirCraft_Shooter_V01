using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boundaryPadding = 0.5f;

    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    // Components
    private HealthSystem healthSystem;
    private EnergySystem energySystem;
    private SpecialAttackSystem specialAttackSystem;

    private void Awake()
    {
        mainCamera = Camera.main;
        CalculateBounds();

        // Get components
        healthSystem = GetComponent<HealthSystem>();
        energySystem = GetComponent<EnergySystem>();
        specialAttackSystem = GetComponent<SpecialAttackSystem>();
    }

    private void CalculateBounds()
    {
        if (mainCamera == null) return;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        minBounds = bottomLeft + new Vector3(boundaryPadding, boundaryPadding, 0);
        maxBounds = topRight - new Vector3(boundaryPadding, boundaryPadding, 0);
    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();
        HandleSpecialAttack();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;

        // Clamp position within bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        transform.position = newPosition;
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Shooting logic will be handled by WeaponSystem
            // Tạm thời để trống
        }
    }

    private void HandleSpecialAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LaserBeam laserBeam = GetComponent<LaserBeam>();
            if (laserBeam != null && !laserBeam.IsActive())
            {
                laserBeam.ActivateLaser();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            LaserBeam laserBeam = GetComponent<LaserBeam>();
            if (laserBeam != null && laserBeam.IsActive())
            {
                laserBeam.DeactivateLaser();
            }
        }
    }

    public void Initialize(ShipData shipData)
    {
        if (shipData != null)
        {
            moveSpeed = shipData.moveSpeed;
        }

        if (healthSystem != null && shipData != null)
            healthSystem.Initialize(shipData.maxHealth);

        if (energySystem != null && shipData != null)
            energySystem.Initialize(shipData.maxEnergy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            if (healthSystem != null)
            {
                // Giả sử bullet có component DamageDealer
                DamageDealer damageDealer = collision.GetComponent<DamageDealer>();
                if (damageDealer != null)
                {
                    healthSystem.TakeDamage(damageDealer.damage);
                }
                else
                {
                    healthSystem.TakeDamage(10); // Default damage
                }
            }
            Destroy(collision.gameObject);
        }

        // Item collection
        if (collision.CompareTag("HealthItem"))
        {
            if (healthSystem != null)
            {
                healthSystem.Heal(25);
            }
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("EnergyItem"))
        {
            if (energySystem != null)
            {
                energySystem.AddEnergy(30);
            }
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("LifeItem"))
        {
            LivesSystem livesSystem = GetComponent<LivesSystem>();
            if (livesSystem != null)
            {
                livesSystem.GainLife();
            }
            Destroy(collision.gameObject);
        }
    }
}