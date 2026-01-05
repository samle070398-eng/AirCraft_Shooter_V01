using UnityEngine;
using UnityEngine.Events;

public class LivesSystem : MonoBehaviour
{
    [System.Serializable]
    public class LivesEvent : UnityEvent<int> { }

    public LivesEvent OnLivesChanged = new LivesEvent();
    public UnityEvent OnNoLivesLeft = new UnityEvent();

    private int currentLives;
    private int maxLives = 3;

    public void Initialize(int startingLives)
    {
        maxLives = startingLives;
        currentLives = startingLives;
        OnLivesChanged?.Invoke(currentLives);
    }

    public void LoseLife()
    {
        currentLives--;
        currentLives = Mathf.Max(0, currentLives);
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            OnNoLivesLeft?.Invoke();
        }
    }

    public void GainLife()
    {
        currentLives++;
        currentLives = Mathf.Min(currentLives, maxLives);
        OnLivesChanged?.Invoke(currentLives);
    }

    public int GetCurrentLives() => currentLives;
    public bool HasLives() => currentLives > 0;
}