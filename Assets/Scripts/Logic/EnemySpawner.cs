using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Enemy enemyPrefab;
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
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = _player.position + (Vector3)(dir * spawnRadius);
        var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, enemyContainer);
        enemy.Initialize(_player);
    }
}
