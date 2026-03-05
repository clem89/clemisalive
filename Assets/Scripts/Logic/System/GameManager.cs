using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameOverUI gameOverUI;
    [SerializeField] LevelUpUI levelUpUI;

    public float SurvivalTime { get; private set; }
    public int KillCount { get; private set; }
    public Character Player { get; private set; }
    public LevelSystem PlayerLevel { get; private set; }

    bool _isGameOver;

    void Awake()
    {
        Instance = this;
        Player = FindFirstObjectByType<Character>();
        PlayerLevel = Player.GetComponent<LevelSystem>();
    }

    void Update()
    {
        if (!_isGameOver)
            SurvivalTime += Time.deltaTime;
    }

    public void OnEnemyKilled()
    {
        KillCount++;
        PlayerLevel.AddXP(10);
    }

    public void TriggerLevelUp()
    {
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
