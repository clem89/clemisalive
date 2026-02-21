using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Enemy enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] float spawnRadius = 12f;       // 플레이어 기준 스폰 거리
    [SerializeField] float initialInterval = 2f;    // 초기 스폰 간격 (초)
    [SerializeField] float minInterval = 0.3f;      // 최소 스폰 간격
    [SerializeField] float difficultyRate = 0.02f;  // 초당 간격 감소량

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

        // 시간이 지날수록 스폰 간격 단축 (난이도 상승)
        _currentInterval = Mathf.Max(minInterval, _currentInterval - difficultyRate * Time.deltaTime);

        if (Time.time >= _nextSpawnTime)
        {
            SpawnEnemy();
            _nextSpawnTime = Time.time + _currentInterval;
        }
    }

    void SpawnEnemy()
    {
        // 플레이어 주변 원 위의 랜덤 위치에 스폰
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = _player.position + (Vector3)(dir * spawnRadius);
        var enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.Initialize(_player);
    }
}
