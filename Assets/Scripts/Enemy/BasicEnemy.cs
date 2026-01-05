using UnityEngine;

public class BasicEnemy : EnemyBase
{
    protected override void Move()
    {
        // Move straight down
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }
}