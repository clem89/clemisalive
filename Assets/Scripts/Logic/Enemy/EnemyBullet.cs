using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] int damage = 8;
    [SerializeField] float lifetime = 5f;

    float _rtSpeed;
    int _rtDamage;
    Vector2 _direction;

    public void Init(int dmg, float spd, Vector2 direction)
    {
        _rtDamage = dmg;
        _rtSpeed = spd;
        _direction = direction.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(_direction * _rtSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Character>(out var player))
        {
            player.TakeDamage(_rtDamage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
