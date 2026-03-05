using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected int maxHealth = 30;
    [SerializeField] protected int contactDamage = 10;
    [SerializeField] protected float damageInterval = 0.8f;

    protected int currentHealth;
    protected float nextDamageTime;
    protected Transform player;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        else if (knockbackDir != Vector2.zero)
        {
            rb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
        }
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
        GameManager.Instance.OnEnemyKilled();
        Destroy(gameObject);
    }
}
