using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [System.Serializable]
    public class HealthEvent : UnityEvent<int, int> { }

    public HealthEvent OnHealthChanged = new HealthEvent();
    public UnityEvent OnDeath = new UnityEvent();

    private int currentHealth;
    private int maxHealth;

    public void Initialize(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}