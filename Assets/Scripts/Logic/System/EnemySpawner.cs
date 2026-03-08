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
    public static EnemySpawner Instance { get; private set; }

    [Header("Enemy Types")]
    [SerializeField] List<EnemySpawnEntry> enemyTypes;

    [Header("Reference")]
    [SerializeField] Transform enemyContainer;

    [Header("Spawn Settings")]
    [SerializeField] float spawnRadius = 12f;
    [SerializeField] float initialInterval = 2f;
    [SerializeField] float minInterval = 0.3f;
    [SerializeField] float difficultyRate = 0.02f;
    [SerializeField] int maxActiveEnemies = 50;

    Transform _player;
    float _currentInterval;
    float _nextSpawnTime;
    readonly List<EnemySpawnEntry> _validEnemies = new();

    // Pooling
    readonly Dictionary<string, Queue<EnemyBase>> _pools = new();
    readonly Dictionary<int, string> _instanceToPoolKey = new();

    bool _isBossActive;

    void Awake()
    {
        Instance = this;
    }

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

    public void SetBossActive(bool active) => _isBossActive = active;

    void SpawnEnemy()
    {
        if (_isBossActive) return;

        // Check max enemy limit
        if (_instanceToPoolKey.Count >= maxActiveEnemies) return;

        float elapsed = Time.time;
        int totalWeight = 0;
        _validEnemies.Clear();

        foreach (var e in enemyTypes)
        {
            if (e.prefab != null && elapsed >= e.unlockTime)
            {
                totalWeight += e.weight;
                _validEnemies.Add(e);
            }
        }

        if (totalWeight <= 0) return;

        int roll = Random.Range(0, totalWeight);
        int accumulated = 0;
        EnemySpawnEntry chosen = _validEnemies[0];

        foreach (var e in _validEnemies)
        {
            accumulated += e.weight;
            if (roll < accumulated) { chosen = e; break; }
        }

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = _player.position + (Vector3)(dir * spawnRadius);
        GetEnemy(chosen.prefab, spawnPos, _player);
    }

    // Pooling methods
    public EnemyBase GetEnemy(EnemyBase prefab, Vector3 position, Transform player)
    {
        string poolKey = prefab.GetType().Name;

        if (!_pools.ContainsKey(poolKey))
            _pools[poolKey] = new Queue<EnemyBase>();

        var pool = _pools[poolKey];
        EnemyBase enemy;

        if (pool.Count == 0)
        {
            enemy = Instantiate(prefab, enemyContainer);
        }
        else
        {
            enemy = pool.Dequeue();
        }

        enemy.transform.position = position;
        enemy.gameObject.SetActive(true);
        enemy.Initialize(player);

        _instanceToPoolKey[enemy.GetInstanceID()] = poolKey;

        return enemy;
    }

    public void ReturnEnemy(EnemyBase enemy)
    {
        if (enemy == null) return;

        int instanceId = enemy.GetInstanceID();

        if (!_instanceToPoolKey.ContainsKey(instanceId))
        {
            Destroy(enemy.gameObject);
            return;
        }

        string poolKey = _instanceToPoolKey[instanceId];

        // Remove from active tracking
        _instanceToPoolKey.Remove(instanceId);

        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(enemyContainer);

        if (!_pools.ContainsKey(poolKey))
            _pools[poolKey] = new Queue<EnemyBase>();

        _pools[poolKey].Enqueue(enemy);
    }
}
