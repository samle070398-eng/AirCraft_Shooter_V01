using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShipSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class ShipData
    {
        public GameObject shipPrefab;
        public string shipName;
        public int health;
        public int damage;
        public float speed;
        public Sprite shipSprite;
    }

    [SerializeField] private ShipData[] ships;
    [SerializeField] private Image shipImage;
    [SerializeField] private TMP_Text shipNameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text speedText;

    private int currentIndex = 0;

    private void Start()
    {
        UpdateShipDisplay();
    }

    private void UpdateShipDisplay()
    {
        ShipData currentShip = ships[currentIndex];
        shipImage.sprite = currentShip.shipSprite;
        shipNameText.text = currentShip.shipName;
        healthText.text = "Health: " + currentShip.health;
        damageText.text = "Damage: " + currentShip.damage;
        speedText.text = "Speed: " + currentShip.speed;
    }

    public void NextShip()
    {
        currentIndex = (currentIndex + 1) % ships.Length;
        UpdateShipDisplay();
    }

    public void PreviousShip()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = ships.Length - 1;
        UpdateShipDisplay();
    }

    public void SelectShip()
    {
        // Lưu thông tin tàu được chọn, có thể dùng ScriptableObject hoặc PlayerPrefs
        PlayerPrefs.SetInt("SelectedShip", currentIndex);
        // Chuyển sang scene Stage1
        SceneManager.LoadScene("Stage1");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}