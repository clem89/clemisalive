using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudUI : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider xpSlider;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI levelText;

    void Update()
    {
        var player = GameManager.Instance.Player;
        hpSlider.value = (float)player.CurrentHealth / player.MaxHealth;

        float t = GameManager.Instance.SurvivalTime;
        int min = (int)t / 60;
        int sec = (int)t % 60;
        timerText.text = $"{min:00}:{sec:00}";

        var lvl = GameManager.Instance.PlayerLevel;
        xpSlider.value = (float)lvl.CurrentXP / lvl.XpToNextLevel;
        levelText.text = "Lv." + lvl.Level;
    }
}
