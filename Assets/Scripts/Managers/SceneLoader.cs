using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Text loadingText;
    [SerializeField] private Text stageText;
    [SerializeField] private Image stageImage;

    [Header("Stage Images")]
    [SerializeField] private Sprite[] stageSprites;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName, bool showLoadingScreen = true)
    {
        StartCoroutine(LoadSceneAsync(sceneName, showLoadingScreen));
    }

    public void LoadSceneWithStageInfo(string sceneName, int stageNumber)
    {
        if (stageText != null && stageNumber > 0)
        {
            stageText.text = $"STAGE {stageNumber}";
        }

        if (stageImage != null && stageSprites.Length > 0)
        {
            int spriteIndex = Mathf.Clamp(stageNumber - 1, 0, stageSprites.Length - 1);
            stageImage.sprite = stageSprites[spriteIndex];
        }

        LoadScene(sceneName);
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool showLoadingScreen)
    {
        if (showLoadingScreen && loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Save player data if exists
        GameManager.Instance?.SavePlayerData();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float progress = 0;

        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }

            if (loadingText != null)
            {
                loadingText.text = $"LOADING... {Mathf.Round(progress * 100)}%";
            }

            if (operation.progress >= 0.9f)
            {
                // Small delay before allowing activation
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }

    public void RestartCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    // Static method for easy access
    public static void LoadSceneStatic(string sceneName)
    {
        if (Instance != null)
        {
            Instance.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}