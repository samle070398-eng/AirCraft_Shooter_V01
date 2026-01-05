using UnityEngine;

public class ItemDropSystem : MonoBehaviour
{
    [System.Serializable]
    public class ItemDrop
    {
        public GameObject itemPrefab;
        [Range(0f, 1f)] public float dropChance;
        public string itemType; // "Health", "Energy", "Life", "Special"
    }

    [Header("Drop Settings")]
    [SerializeField] private ItemDrop[] itemDrops;
    [SerializeField] private float totalDropChance = 1f;
    [SerializeField] private bool useGlobalSettings = true;

    private void Start()
    {
        if (useGlobalSettings && GameManager.Instance != null && GameManager.Instance.Settings != null)
        {
            // Override drop chances with global settings if needed
            UpdateDropChancesFromSettings();
        }

        // Normalize drop chances
        NormalizeDropChances();
    }

    private void UpdateDropChancesFromSettings()
    {
        foreach (ItemDrop item in itemDrops)
        {
            switch (item.itemType)
            {
                case "Health":
                    item.dropChance = GameManager.Instance.Settings.healthDropChance;
                    break;
                case "Energy":
                    item.dropChance = GameManager.Instance.Settings.energyDropChance;
                    break;
                case "Life":
                    item.dropChance = GameManager.Instance.Settings.lifeDropChance;
                    break;
            }
        }
    }

    private void NormalizeDropChances()
    {
        float currentTotal = 0f;
        foreach (ItemDrop item in itemDrops)
        {
            currentTotal += item.dropChance;
        }

        if (currentTotal > totalDropChance)
        {
            float multiplier = totalDropChance / currentTotal;
            foreach (ItemDrop item in itemDrops)
            {
                item.dropChance *= multiplier;
            }
        }
    }

    public void DropItem(Vector3 position)
    {
        if (itemDrops.Length == 0) return;

        float randomValue = Random.value;
        float accumulatedChance = 0f;

        foreach (ItemDrop item in itemDrops)
        {
            accumulatedChance += item.dropChance;

            if (randomValue <= accumulatedChance)
            {
                if (item.itemPrefab != null)
                {
                    GameObject droppedItem = Instantiate(item.itemPrefab, position, Quaternion.identity);

                    // Add floating animation
                    FloatingItem floating = droppedItem.AddComponent<FloatingItem>();
                    floating.floatAmplitude = 0.5f;
                    floating.floatSpeed = 2f;

                    // Auto-collect after time
                    AutoCollect autoCollect = droppedItem.AddComponent<AutoCollect>();
                    autoCollect.collectTime = 10f;

                    break;
                }
            }
        }
    }

    public void DropRandomItem(Vector3 position)
    {
        DropItem(position);
    }
}

// Floating animation for items
public class FloatingItem : MonoBehaviour
{
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 2f;

    private Vector3 startPosition;
    private float floatTimer = 0f;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        floatTimer += Time.deltaTime;

        float yOffset = Mathf.Sin(floatTimer * floatSpeed) * floatAmplitude;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }
}

// Auto-collect items after time
public class AutoCollect : MonoBehaviour
{
    public float collectTime = 10f;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= collectTime)
        {
            // Fade out effect
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private System.Collections.IEnumerator FadeOutAndDestroy()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            float fadeTime = 1f;
            float elapsed = 0f;
            Color startColor = renderer.color;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                renderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}