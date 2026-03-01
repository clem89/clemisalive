using UnityEngine;

public enum UpgradeType
{
    MaxHp, HpRestore, MoveSpeed, FireRate,
    BulletDamage, BulletSpeed, Pierce, DashCooldown
}

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance { get; private set; }

    [SerializeField] int xpPerKill = 10;

    int _level = 1;
    int _currentXP = 0;
    int _xpToNextLevel = 50;

    public int Level => _level;
    public int CurrentXP => _currentXP;
    public int XpToNextLevel => _xpToNextLevel;

    void Awake()
    {
        Instance = this;
    }

    public void AddXP(int amount)
    {
        _currentXP += amount;
        while (_currentXP >= _xpToNextLevel)
        {
            _currentXP -= _xpToNextLevel;
            _xpToNextLevel += 20;
            _level++;
            GameManager.Instance.TriggerLevelUp();
        }
    }
}
