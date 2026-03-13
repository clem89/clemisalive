using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossWarningUI : MonoBehaviour
{
    static BossWarningUI _instance;
    public static BossWarningUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<BossWarningUI>(FindObjectsInactive.Include);
            return _instance;
        }
    }

    [Header("Warning Text")]
    [SerializeField] TextMeshProUGUI warningText;
    [SerializeField] string warningMessage = "! BOSS INCOMING !";
    [SerializeField] float displayDuration = 2.5f;
    [SerializeField] float fadeDuration = 0.4f;

    [Header("Screen Vignette")]
    [SerializeField] Image vignetteImage;
    [SerializeField] float vignetteFlashCount = 4f;

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        if (warningText != null)
        {
            warningText.text = warningMessage;
            warningText.alpha = 0f;
        }
        if (vignetteImage != null)
            vignetteImage.color = new Color(1f, 0f, 0f, 0f);

        yield return StartCoroutine(FadeText(0f, 1f, fadeDuration));

        float elapsed = 0f;
        float flashInterval = displayDuration / (vignetteFlashCount * 2);
        bool vignetteOn = false;

        while (elapsed < displayDuration)
        {
            vignetteOn = !vignetteOn;
            if (vignetteImage != null)
                vignetteImage.color = new Color(1f, 0f, 0f, vignetteOn ? 0.35f : 0f);

            elapsed += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        if (vignetteImage != null)
            vignetteImage.color = new Color(1f, 0f, 0f, 0f);

        yield return StartCoroutine(FadeText(1f, 0f, fadeDuration));

        gameObject.SetActive(false);
    }

    IEnumerator FadeText(float from, float to, float duration)
    {
        if (warningText == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            warningText.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        warningText.alpha = to;
    }
}
