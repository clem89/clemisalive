using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameOverUI gameOverUI;
    [SerializeField] LevelUpUI levelUpUI;

    [Header("Boss Settings")]
    [SerializeField] BossEnemy bossPrefab;
    [SerializeField] float bossSpawnInterval = 60f;
    [SerializeField] int bossKillXpReward = 100;

    public float SurvivalTime { get; private set; }
    public int KillCount { get; private set; }
    public Character Player { get; private set; }
    public LevelSystem PlayerLevel { get; private set; }

    bool _isGameOver;
    float _nextBossTime;
    BossEnemy _activeBoss;

    void Awake()
    {
        Instance = this;
        Player = FindFirstObjectByType<Character>();
        PlayerLevel = Player.GetComponent<LevelSystem>();
    }

    void Start()
    {
        _nextBossTime = bossSpawnInterval;
    }

    void Update()
    {
        if (!_isGameOver)
        {
            SurvivalTime += Time.deltaTime;
            CheckBossSpawn();
        }
    }

    void CheckBossSpawn()
    {
        if (_activeBoss != null) return;
        if (bossPrefab == null) return;
        if (SurvivalTime >= _nextBossTime)
        {
            SpawnBoss();
            _nextBossTime += bossSpawnInterval;
        }
    }

    void SpawnBoss()
    {
        EnemySpawner.Instance?.SetBossActive(true);
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = Player.transform.position + (Vector3)(dir * 12f);
        _activeBoss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        _activeBoss.Initialize(Player.transform);
        HudUI.Instance?.ShowBossHp(_activeBoss);
    }

    public void OnBossKilled()
    {
        _activeBoss = null;
        EnemySpawner.Instance?.SetBossActive(false);
        HudUI.Instance?.HideBossHp();
        KillCount++;
        PlayerLevel.AddXP(bossKillXpReward);
    }

    public void OnEnemyKilled()
    {
        KillCount++;
        PlayerLevel.AddXP(10);
    }

    public void TriggerLevelUp()
    {
        // Spawn level up effect
        if (Player != null)
            LevelUpEffect.Create(Player.transform.position);

        Time.timeScale = 0f;
        var options = PickRandomUpgrades(3);
        levelUpUI.Show(options);
    }

    public void OnUpgradeSelected(UpgradeType type)
    {
        Player.ApplyUpgrade(type);
        Time.timeScale = 1f;
        levelUpUI.Hide();
    }

    public void OnPlayerDied()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        Time.timeScale = 0f;
        gameOverUI.Show(SurvivalTime, KillCount);
    }

    UpgradeType[] PickRandomUpgrades(int count)
    {
        var allTypes = (UpgradeType[])System.Enum.GetValues(typeof(UpgradeType));
        int n = allTypes.Length;
        var shuffled = new UpgradeType[n];
        System.Array.Copy(allTypes, shuffled, n);

        // Fisher-Yates shuffle
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        var result = new UpgradeType[count];
        System.Array.Copy(shuffled, result, count);
        return result;
    }
}
