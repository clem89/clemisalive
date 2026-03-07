using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("Ranged Settings")]
    [SerializeField] EnemyBullet bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] int bulletDamage = 8;
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float preferredDistance = 6f;
    [SerializeField] float distanceThreshold = 0.5f;
    [SerializeField] float fireInterval = 2f;

    float _nextFireTime;

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float distance = toPlayer.magnitude;
        Vector2 dir = toPlayer.normalized;

        // Face the player
        rb.SetRotation(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);

        // Move to maintain preferred distance
        if (distance > preferredDistance + distanceThreshold)
        {
            rb.linearVelocity = dir * moveSpeed;
        }
        else if (distance < preferredDistance - distanceThreshold)
        {
            rb.linearVelocity = -dir * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (player == null) return;
        if (Time.time < _nextFireTime) return;

        Fire();
        _nextFireTime = Time.time + fireInterval;
    }

    void Fire()
    {
        Transform spawnPoint = firePoint != null ? firePoint : transform;
        Vector2 dir = ((Vector2)player.position - (Vector2)spawnPoint.position).normalized;

        EnemyBulletPool.Instance?.Get(spawnPoint.position, bulletDamage, bulletSpeed, dir);
    }

    protected override void OnReturnToPool()
    {
        base.OnReturnToPool();
        _nextFireTime = 0f;
        rb.linearVelocity = Vector2.zero;
    }
}
