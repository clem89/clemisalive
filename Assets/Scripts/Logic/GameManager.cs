using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameOverUI gameOverUI;

    public float SurvivalTime { get; private set; }
    public int KillCount { get; private set; }
    public Character Player { get; private set; }

    bool _isGameOver;

    void Awake()
    {
        Instance = this;
        Player = FindFirstObjectByType<Character>();
    }

    void Update()
    {
        if (!_isGameOver)
            SurvivalTime += Time.deltaTime;
    }

    public void AddKill()
    {
        KillCount++;
    }

    public void OnPlayerDied()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        Time.timeScale = 0f;
        gameOverUI.Show(SurvivalTime, KillCount);
    }
}
