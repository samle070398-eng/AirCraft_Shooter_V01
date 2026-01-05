using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("ShipSelection");
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}