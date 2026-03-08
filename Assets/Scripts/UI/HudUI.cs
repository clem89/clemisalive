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

    [Header("Boss HP")]
    [SerializeField] GameObject bossHpGroup;
    [SerializeField] Slider bossHpSlider;
    [SerializeField] TextMeshProUGUI bossHpText;

    Vector3 _hpSliderOriginalPos;
    BossEnemy _trackedBoss;

    void Awake()
    {
        Instance = this;
        if (hpSlider != null)
            _hpSliderOriginalPos = hpSlider.transform.localPosition;
        if (bossHpGroup != null)
            bossHpGroup.SetActive(false);
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

        if (_trackedBoss != null && bossHpSlider != null)
        {
            float ratio = (float)_trackedBoss.CurrentHealth / _trackedBoss.MaxHealth;
            bossHpSlider.value = ratio;
            if (bossHpText != null)
                bossHpText.text = $"{_trackedBoss.CurrentHealth} / {_trackedBoss.MaxHealth}";
        }
    }

    public void ShowBossHp(BossEnemy boss)
    {
        _trackedBoss = boss;
        if (bossHpGroup != null) bossHpGroup.SetActive(true);
    }

    public void HideBossHp()
    {
        _trackedBoss = null;
        if (bossHpGroup != null) bossHpGroup.SetActive(false);
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
