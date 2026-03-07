using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HudUI : MonoBehaviour
{
    public static HudUI Instance { get; private set; }

    [SerializeField] Slider hpSlider;
    [SerializeField] Slider xpSlider;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI levelText;

    [Header("Shake Settings")]
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float shakeMagnitude = 10f;

    Vector3 _hpSliderOriginalPos;

    void Awake()
    {
        Instance = this;
        if (hpSlider != null)
            _hpSliderOriginalPos = hpSlider.transform.localPosition;
    }

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

    public void ShakeHealthBar()
    {
        if (hpSlider != null)
            StartCoroutine(ShakeHealthBarCoroutine());
    }

    IEnumerator ShakeHealthBarCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = Random.Range(-shakeMagnitude, shakeMagnitude);

            hpSlider.transform.localPosition = _hpSliderOriginalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        hpSlider.transform.localPosition = _hpSliderOriginalPos;
    }
}
