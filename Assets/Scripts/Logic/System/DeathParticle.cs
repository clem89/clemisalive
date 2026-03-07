using UnityEngine;

public class DeathParticle : MonoBehaviour
{
    [SerializeField] float lifetime = 0.5f;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float gravity = -5f;

    Vector2 _velocity;
    float _spawnTime;
    SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Vector2 velocity, Color color)
    {
        _velocity = velocity;
        _spawnTime = Time.time;
        if (_spriteRenderer != null)
            _spriteRenderer.color = color;
    }

    void Update()
    {
        // Move
        _velocity.y += gravity * Time.deltaTime;
        transform.position += (Vector3)_velocity * Time.deltaTime;

        // Fade out
        float elapsed = Time.time - _spawnTime;
        float alpha = 1f - (elapsed / lifetime);

        if (_spriteRenderer != null)
        {
            Color c = _spriteRenderer.color;
            c.a = alpha;
            _spriteRenderer.color = c;
        }

        // Destroy when lifetime expires
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
