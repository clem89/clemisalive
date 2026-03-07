using UnityEngine;

public class LevelUpEffect : MonoBehaviour
{
    [SerializeField] float duration = 0.5f;
    [SerializeField] float maxScale = 3f;
    [SerializeField] Color effectColor = Color.yellow;

    float _startTime;
    SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startTime = Time.unscaledTime;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = effectColor;
        }
    }

    void Update()
    {
        float elapsed = Time.unscaledTime - _startTime;
        float progress = elapsed / duration;

        if (progress >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        // Scale up
        float scale = Mathf.Lerp(0.5f, maxScale, progress);
        transform.localScale = Vector3.one * scale;

        // Fade out
        if (_spriteRenderer != null)
        {
            Color c = _spriteRenderer.color;
            c.a = 1f - progress;
            _spriteRenderer.color = c;
        }
    }

    public static void Create(Vector3 position)
    {
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ring.transform.position = position;
        ring.transform.localScale = Vector3.one * 0.5f;

        // Remove collider
        Destroy(ring.GetComponent<Collider>());

        // Make it a ring by scaling it flat
        ring.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);

        ring.AddComponent<LevelUpEffect>();
    }
}
