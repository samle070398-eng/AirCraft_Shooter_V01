using UnityEngine;

public static class ResourceLoader
{
    // Load GameSettings from Resources
    public static GameSettings LoadGameSettings()
    {
        GameSettings settings = Resources.Load<GameSettings>("Settings/GameSettings");

        if (settings == null)
        {
            Debug.LogError("GameSettings not found in Resources/Settings/GameSettings! Creating default...");

            // Create default settings
            settings = ScriptableObject.CreateInstance<GameSettings>();

#if UNITY_EDITOR
            // Save asset in editor
            string path = "Assets/Resources/Settings/GameSettings.asset";
            UnityEditor.AssetDatabase.CreateAsset(settings, path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        return settings;
    }

    // Load ShipData from Resources
    public static ShipData[] LoadAllShipData()
    {
        ShipData[] ships = Resources.LoadAll<ShipData>("Ships");

        if (ships == null || ships.Length == 0)
        {
            Debug.LogError("No ShipData found in Resources/Ships/! Creating defaults...");
            ships = CreateDefaultShips();
        }

        return ships;
    }

    private static ShipData[] CreateDefaultShips()
    {
        ShipData[] defaultShips = new ShipData[3];

        for (int i = 0; i < 3; i++)
        {
            defaultShips[i] = ScriptableObject.CreateInstance<ShipData>();
            defaultShips[i].shipName = $"Ship {i + 1}";
            defaultShips[i].description = $"Default ship {i + 1}";

#if UNITY_EDITOR
            string path = $"Assets/Resources/Ships/Ship_{i + 1}.asset";
            UnityEditor.AssetDatabase.CreateAsset(defaultShips[i], path);
#endif
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
#endif

        return defaultShips;
    }
}