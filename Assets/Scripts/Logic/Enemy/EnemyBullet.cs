using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] int damage = 8;
    [SerializeField] float lifetime = 5f;

    float _rtSpeed;
    int _rtDamage;
    Vector2 _direction;
    float _spawnTime;

    public void Init(int dmg, float spd, Vector2 direction)
    {
        _rtDamage = dmg;
        _rtSpeed = spd;
        _direction = direction.normalized;
        _spawnTime = Time.time;
    }

    void Update()
    {
        transform.Translate(_direction * _rtSpeed * Time.deltaTime, Space.World);

        // Check lifetime
        if (Time.time - _spawnTime >= lifetime)
        {
            ReturnToPool();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Character>(out var player))
        {
            player.TakeDamage(_rtDamage);
            ReturnToPool();
        }
        else if (other.CompareTag("Wall"))
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        EnemyBulletPool.Instance?.Return(this);
    }
}
