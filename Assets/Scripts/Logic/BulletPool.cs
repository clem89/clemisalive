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
        // 풀이 비었으면 자동 확장
        if (_pool.Count == 0) CreateBullet();

        var bullet = _pool.Dequeue();
        // SetPositionAndRotation은 항상 월드 좌표 → 부모 아래 있어도 정상 동작
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void Return(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(transform);
        _pool.Enqueue(bullet);
    }
}
