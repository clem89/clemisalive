using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 12f;
    [SerializeField] int damage = 10;
    [SerializeField] float lifetime = 3f;
    [SerializeField] int pierceCount = 0;

    int _rtDamage;
    float _rtSpeed;
    int _rtPierce;
    int _pierceRemaining;
    float _expireTime;

    void OnEnable()
    {
        _rtDamage = damage;
        _rtSpeed = speed;
        _rtPierce = pierceCount;
        _pierceRemaining = _rtPierce;
        _expireTime = Time.time + lifetime;
    }

    public void Init(int dmg, float spd, int pierce)
    {
        _rtDamage = dmg;
        _rtSpeed = spd;
        _rtPierce = pierce;
        _pierceRemaining = pierce;
    }

    void Update()
    {
        transform.Translate(Vector2.up * _rtSpeed * Time.deltaTime);

        if (Time.time >= _expireTime)
            ReturnToPool();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(_rtDamage);
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
