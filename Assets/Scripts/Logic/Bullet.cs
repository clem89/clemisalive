using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 12f;
    [SerializeField] int damage = 10;
    [SerializeField] float lifetime = 3f;
    [SerializeField] int pierceCount = 0;

    int _pierceRemaining;
    float _expireTime;

    void OnEnable()
    {
        _pierceRemaining = pierceCount;
        _expireTime = Time.time + lifetime;
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        if (Time.time >= _expireTime)
            ReturnToPool();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(damage);
            if (_pierceRemaining <= 0)
                ReturnToPool();
            else
                _pierceRemaining--;
        }
        else if (other.CompareTag("Wall"))
        {
            ReturnToPool();
        }
    }

    void ReturnToPool() => BulletPool.Instance.Return(this);
}
