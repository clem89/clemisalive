using UnityEngine;

public class SummonerEnemy : EnemyBase
{
    [Header("Summoner Settings")]
    [SerializeField] float moveSpeed = 0.8f;
    [SerializeField] float summonInterval = 4f;
    [SerializeField] float summonRadius = 1.5f;
    [SerializeField] Enemy minionPrefab;

    float _nextSummonTime;

    protected override void Awake()
    {
        maxHealth = 80;
        base.Awake();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
        rb.SetRotation(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);
    }

    void Update()
    {
        if (player == null) return;
        if (Time.time < _nextSummonTime) return;

        SummonMinions();
        _nextSummonTime = Time.time + summonInterval;
    }

    void SummonMinions()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector2 offset = Random.insideUnitCircle * summonRadius;
            Vector3 spawnPos = transform.position + (Vector3)offset;
            var minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity, transform.parent);
            minion.Initialize(player);
        }
    }
}
