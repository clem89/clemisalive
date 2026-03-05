using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnEntry
{
    public EnemyBase prefab;
    [Range(1, 100)] public int weight = 50;
    [Tooltip("이 적이 등장하기 시작하는 경과 시간(초)")]
    public float unlockTime = 0f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Types")]
    [SerializeField] List<EnemySpawnEntry> enemyTypes;

    [Header("Reference")]
    [SerializeField] Transform enemyContainer;

    [Header("Spawn Settings")]
    [SerializeField] float spawnRadius = 12f;
    [SerializeField] float initialInterval = 2f;
    [SerializeField] float minInterval = 0.3f;
    [SerializeField] float difficultyRate = 0.02f;

    Transform _player;
    float _currentInterval;
    float _nextSpawnTime;

    void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;

        _currentInterval = initialInterval;
        _nextSpawnTime = Time.time + _currentInterval;
    }

    void Update()
    {
        if (_player == null) return;

        _currentInterval = Mathf.Max(minInterval, _currentInterval - difficultyRate * Time.deltaTime);

        if (Time.time >= _nextSpawnTime)
        {
            SpawnEnemy();
            _nextSpawnTime = Time.time + _currentInterval;
        }
    }

    void SpawnEnemy()
    {
        float elapsed = Time.time;
        int totalWeight = 0;
        var valid = new List<EnemySpawnEntry>();

        foreach (var e in enemyTypes)
        {
            if (e.prefab != null && elapsed >= e.unlockTime)
            {
                totalWeight += e.weight;
                valid.Add(e);
            }
        }

        if (totalWeight <= 0) return;

        int roll = Random.Range(0, totalWeight);
        int accumulated = 0;
        EnemySpawnEntry chosen = valid[0];

        foreach (var e in valid)
        {
            accumulated += e.weight;
            if (roll < accumulated) { chosen = e; break; }
        }

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = _player.position + (Vector3)(dir * spawnRadius);
        var enemy = Instantiate(chosen.prefab, spawnPos, Quaternion.identity, enemyContainer);
        enemy.Initialize(_player);
    }
}
