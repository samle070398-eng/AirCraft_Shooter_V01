using UnityEngine;

public class FastEnemy : EnemyBase
{
    [SerializeField] private float diveDistance = 3f;
    [SerializeField] private float diveSpeedMultiplier = 2f;

    private bool hasDived = false;
    private Vector3 diveTarget;

    protected override void Start()
    {
        base.Start();
        if (playerTransform != null)
        {
            diveTarget = playerTransform.position;
        }
    }

    protected override void Move()
    {
        if (!hasDived && Vector3.Distance(transform.position, diveTarget) < diveDistance)
        {
            hasDived = true;
        }

        if (hasDived)
        {
            // Dive towards player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.Translate(direction * speed * diveSpeedMultiplier * Time.deltaTime);
        }
        else
        {
            // Move straight down
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }
}