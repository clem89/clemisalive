using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int maxHealth = 30;
    [SerializeField] float moveSpeed = 2.5f;
    [SerializeField] int contactDamage = 10;
    [SerializeField] float damageInterval = 0.8f;

    int _currentHealth;
    float _nextDamageTime;
    Transform _player;
    Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentHealth = maxHealth;
    }

    void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (_player == null) return;
        Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
        _rb.linearVelocity = dir * moveSpeed;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (Time.time < _nextDamageTime) return;
        if (col.gameObject.TryGetComponent<Character>(out var player))
        {
            player.TakeDamage(contactDamage);
            _nextDamageTime = Time.time + damageInterval;
        }
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        // TODO: 히트 플래시 효과
        if (_currentHealth <= 0) Die();
    }

    void Die()
    {
        // TODO: 파티클 + 보상 드랍
        Destroy(gameObject);
    }
}
