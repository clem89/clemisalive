using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI killText;
    [SerializeField] Button restartButton;
    [SerializeField] Button mainMenuButton;

    void Awake()
    {
        restartButton.onClick.AddListener(Restart);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void Show(float survivalTime, int killCount)
    {
        int min = (int)survivalTime / 60;
        int sec = (int)survivalTime % 60;
        timeText.text = $"생존 시간  {min:00}:{sec:00}";
        killText.text = $"처치 수    {killCount}";
        gameObject.SetActive(true);
    }

    void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}
