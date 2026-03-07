using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected int maxHealth = 30;
    [SerializeField] protected int contactDamage = 10;
    [SerializeField] protected float damageInterval = 0.8f;

    [Header("Visual")]
    [SerializeField] protected float hitFlashDuration = 0.1f;
    [SerializeField] protected int deathParticleCount = 8;

    protected int currentHealth;
    protected float nextDamageTime;
    protected Transform player;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    public virtual void Initialize(Transform player)
    {
        this.player = player;
    }

    public void TakeDamage(int amount, Vector2 knockbackDir = default)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (knockbackDir != Vector2.zero)
                rb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);

            StartCoroutine(HitFlash());
        }
    }

    protected IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    protected virtual void OnTriggerStay2D(Collider2D col)
    {
        if (Time.time < nextDamageTime) return;
        if (col.TryGetComponent<Character>(out var p))
        {
            p.TakeDamage(contactDamage);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    protected virtual void Die()
    {
        SpawnDeathParticles();
        GameManager.Instance.OnEnemyKilled();
        OnReturnToPool();
        EnemySpawner.Instance?.ReturnEnemy(this);
    }

    void SpawnDeathParticles()
    {
        Color particleColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        for (int i = 0; i < deathParticleCount; i++)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Quad);
            particle.transform.position = transform.position;
            particle.transform.localScale = Vector3.one * 0.2f;

            // Remove collider from primitive
            Destroy(particle.GetComponent<Collider>());

            // Add DeathParticle component
            var deathParticle = particle.AddComponent<DeathParticle>();

            // Random velocity
            float angle = (360f / deathParticleCount) * i + Random.Range(-15f, 15f);
            Vector2 velocity = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * Random.Range(2f, 4f);

            deathParticle.Init(velocity, particleColor);
        }
    }

    protected virtual void OnReturnToPool()
    {
        // Override in subclasses to reset state
        currentHealth = maxHealth;
        nextDamageTime = 0f;

        // Reset visual
        StopAllCoroutines();
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}
