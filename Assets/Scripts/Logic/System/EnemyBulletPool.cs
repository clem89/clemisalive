using UnityEngine;
using System.Collections.Generic;

public class EnemyBulletPool : MonoBehaviour
{
    public static EnemyBulletPool Instance { get; private set; }

    [SerializeField] EnemyBullet bulletPrefab;

    readonly Queue<EnemyBullet> _pool = new();
    readonly List<EnemyBullet> _activeBullets = new();

    void Awake()
    {
        Instance = this;
    }

    public EnemyBullet Get(Vector3 position, int damage, float speed, Vector2 direction)
    {
        EnemyBullet bullet;

        if (_pool.Count == 0)
        {
            bullet = Instantiate(bulletPrefab, transform);
        }
        else
        {
            bullet = _pool.Dequeue();
        }

        bullet.transform.position = position;
        bullet.gameObject.SetActive(true);
        bullet.Init(damage, speed, direction);

        _activeBullets.Add(bullet);
        return bullet;
    }

    public void Return(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(transform);
        _pool.Enqueue(bullet);
        _activeBullets.Remove(bullet);
    }

    void Update()
    {
        // Check for bullets that need to be returned (lifetime expired)
        for (int i = _activeBullets.Count - 1; i >= 0; i--)
        {
            var bullet = _activeBullets[i];
            if (bullet == null || !bullet.gameObject.activeInHierarchy)
            {
                _activeBullets.RemoveAt(i);
            }
        }
    }
}
