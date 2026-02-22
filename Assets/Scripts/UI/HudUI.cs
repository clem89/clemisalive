using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudUI : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI timerText;

    void Update()
    {
        var player = GameManager.Instance.Player;
        hpSlider.value = (float)player.CurrentHealth / player.MaxHealth;

        float t = GameManager.Instance.SurvivalTime;
        int min = (int)t / 60;
        int sec = (int)t % 60;
        timerText.text = $"{min:00}:{sec:00}";
    }
}
