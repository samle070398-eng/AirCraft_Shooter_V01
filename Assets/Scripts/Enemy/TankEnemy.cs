using UnityEngine;

public class TankEnemy : EnemyBase
{
    [Header("Zigzag Settings")]
    [SerializeField] private float zigzagAmplitude = 2f;
    [SerializeField] private float zigzagFrequency = 2f;

    private float zigzagTimer = 0f;
    private bool movingRight = true;

    protected override void Move()
    {
        // Move down with zigzag pattern
        float verticalMovement = -speed * Time.deltaTime;

        // Calculate zigzag movement
        zigzagTimer += Time.deltaTime;
        float horizontalMovement = 0f;

        if (movingRight)
        {
            horizontalMovement = Mathf.Sin(zigzagTimer * zigzagFrequency) * zigzagAmplitude * Time.deltaTime;
        }
        else
        {
            horizontalMovement = -Mathf.Sin(zigzagTimer * zigzagFrequency) * zigzagAmplitude * Time.deltaTime;
        }

        // Change direction at screen edges
        if (transform.position.x > 8f) movingRight = false;
        if (transform.position.x < -8f) movingRight = true;

        // Apply movement
        transform.Translate(new Vector3(horizontalMovement, verticalMovement, 0));

        // Destroy if out of bounds
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    public override void TakeDamage(int damageAmount)
    {
        // Tank enemy takes reduced damage
        int reducedDamage = Mathf.Max(1, damageAmount / 2);
        base.TakeDamage(reducedDamage);
    }
}