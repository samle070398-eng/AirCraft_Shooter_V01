using UnityEngine;
using UnityEngine.Events;

public class EnergySystem : MonoBehaviour
{
    [System.Serializable]
    public class EnergyEvent : UnityEvent<float, float> { }

    public EnergyEvent OnEnergyChanged = new EnergyEvent();
    public UnityEvent OnEnergyFull = new UnityEvent();

    private float currentEnergy;
    private float maxEnergy;
    private float regenerationRate;

    public void Initialize(float maxEnergy)
    {
        this.maxEnergy = maxEnergy;
        currentEnergy = 0;

        // Get regeneration rate from GameSettings
        if (GameManager.Instance != null && GameManager.Instance.Settings != null)
        {
            regenerationRate = GameManager.Instance.Settings.energyRegenerationRate;
        }
        else
        {
            regenerationRate = 10f; // Default
        }

        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    private void Update()
    {
        RegenerateEnergy();
    }

    private void RegenerateEnergy()
    {
        if (currentEnergy >= maxEnergy) return;

        currentEnergy += regenerationRate * Time.deltaTime;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        if (currentEnergy >= maxEnergy)
        {
            OnEnergyFull?.Invoke();
        }
    }

    public void AddEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    public bool UseEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            return true;
        }
        return false;
    }

    public bool HasEnoughEnergy(float amount) => currentEnergy >= amount;
    public float GetCurrentEnergy() => currentEnergy;
    public float GetMaxEnergy() => maxEnergy;
}