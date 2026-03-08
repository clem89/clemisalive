using UnityEngine;
using System.Collections;

public class BossEnemy : EnemyBase
{
    [Header("Move Settings")]
    [SerializeField] float phase1MoveSpeed = 2.5f;
    [SerializeField] float phase2MoveSpeed = 4.5f;

    [Header("Attack Settings")]
    [SerializeField] EnemyBullet bulletPrefab;
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] int bulletDamage = 15;
    [SerializeField] float fireInterval = 1.5f;

    [Header("Phase Settings")]
    [SerializeField] int bulletCountPhase1 = 3;
    [SerializeField] int bulletCountPhase2 = 8;
    [SerializeField] int bulletCountPhase3 = 16;

    [Header("Phase 3 - Summon")]
    [SerializeField] EnemyBase minionPrefab;
    [SerializeField] float phase3SummonInterval = 4f;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    int _currentPhase = 1;
    float _currentMoveSpeed;
    float _nextFireTime;
    float _nextSummonTime;

    protected override void Awake()
    {
        base.Awake();
        _currentMoveSpeed = phase1MoveSpeed;
    }

    public override void Initialize(Transform playerTransform)
    {
        base.Initialize(playerTransform);
        _currentPhase = 1;
        _currentMoveSpeed = phase1MoveSpeed;
        _nextFireTime = Time.time + 1f;
        _nextSummonTime = float.MaxValue;
    }

    void Update()
    {
        if (player == null) return;

        UpdatePhase();

        if (Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireInterval;
        }

        if (_currentPhase == 3 && Time.time >= _nextSummonTime)
        {
            SummonMinions();
            _nextSummonTime = Time.time + phase3SummonInterval;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * _currentMoveSpeed;
        rb.SetRotation(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);
    }

    void UpdatePhase()
    {
        float hpRatio = (float)currentHealth / maxHealth;
        int newPhase;

        if (hpRatio > 0.5f) newPhase = 1;
        else if (hpRatio > 0.25f) newPhase = 2;
        else newPhase = 3;

        if (newPhase != _currentPhase)
        {
            _currentPhase = newPhase;
            OnPhaseChanged();
        }
    }

    void OnPhaseChanged()
    {
        if (_currentPhase == 2)
        {
            _currentMoveSpeed = phase2MoveSpeed;
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1f, 0.5f, 0.5f);
            originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        }
        else if (_currentPhase == 3)
        {
            _currentMoveSpeed = phase2MoveSpeed;
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1f, 0.2f, 0.2f);
            originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
            _nextSummonTime = Time.time + phase3SummonInterval;
        }
    }

    void Fire()
    {
        if (bulletPrefab == null) return;

        int count = _currentPhase == 1 ? bulletCountPhase1
                  : _currentPhase == 2 ? bulletCountPhase2
                  : bulletCountPhase3;

        float angleStep = 360f / count;
        float startAngle = _currentPhase == 1
            ? Mathf.Atan2(player.position.y - transform.position.y,
                          player.position.x - transform.position.x) * Mathf.Rad2Deg - (angleStep * (count - 1) / 2f)
            : 0f;

        for (int i = 0; i < count; i++)
        {
            float angle = _currentPhase == 1
                ? startAngle + angleStep * i
                : angleStep * i;

            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            EnemyBulletPool.Instance?.Get(transform.position, bulletDamage, bulletSpeed, dir);
        }
    }

    void SummonMinions()
    {
        if (minionPrefab == null || EnemySpawner.Instance == null) return;

        for (int i = 0; i < 2; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * 2f;
            EnemySpawner.Instance.GetEnemy(minionPrefab, transform.position + (Vector3)offset, player);
        }
    }

    protected override void Die()
    {
        SpawnDeathParticles();
        GameManager.Instance.OnBossKilled();
        Destroy(gameObject);
    }

    void SpawnDeathParticles()
    {
        Color particleColor = spriteRenderer != null ? spriteRenderer.color : Color.red;
        for (int i = 0; i < 20; i++)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Quad);
            p.transform.position = transform.position;
            p.transform.localScale = Vector3.one * 0.35f;
            Destroy(p.GetComponent<Collider>());
            var dp = p.AddComponent<DeathParticle>();
            float angle = (360f / 20) * i + Random.Range(-10f, 10f);
            Vector2 vel = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * Random.Range(3f, 6f);
            dp.Init(vel, particleColor);
        }
    }
}
