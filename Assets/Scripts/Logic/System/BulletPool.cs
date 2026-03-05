using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] Bullet bulletPrefab;
    [SerializeField] int poolSize = 20;

    readonly Queue<Bullet> _pool = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
            CreateBullet();
    }

    Bullet CreateBullet()
    {
        var bullet = Instantiate(bulletPrefab, transform);
        bullet.gameObject.SetActive(false);
        _pool.Enqueue(bullet);
        return bullet;
    }

    public Bullet Get(Vector3 position, Quaternion rotation)
    {
        if (_pool.Count == 0) CreateBullet();

        var bullet = _pool.Dequeue();
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public Bullet Get(Vector3 position, Quaternion rotation, int damage, float speed, int pierce)
    {
        var bullet = Get(position, rotation);
        bullet.Init(damage, speed, pierce);
        return bullet;
    }

    public void Return(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(transform);
        _pool.Enqueue(bullet);
    }
}
