using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] TextMeshProUGUI[] buttonLabels;
    [SerializeField] TextMeshProUGUI[] buttonDescs;

    UpgradeType[] _currentOptions;

    public void Show(UpgradeType[] options)
    {
        gameObject.SetActive(true);
        _currentOptions = options;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonLabels[i].text = GetUpgradeName(options[i]);
            buttonDescs[i].text = GetUpgradeDesc(options[i]);

            int idx = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => GameManager.Instance.OnUpgradeSelected(_currentOptions[idx]));
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    string GetUpgradeName(UpgradeType t)
    {
        return t switch
        {
            UpgradeType.MaxHp => "체력 증가",
            UpgradeType.HpRestore => "HP 회복",
            UpgradeType.MoveSpeed => "이동속도 향상",
            UpgradeType.FireRate => "공격속도 향상",
            UpgradeType.BulletDamage => "데미지 강화",
            UpgradeType.BulletSpeed => "총알 속도",
            UpgradeType.Pierce => "관통",
            UpgradeType.DashCooldown => "대시 강화",
            _ => ""
        };
    }

    string GetUpgradeDesc(UpgradeType t)
    {
        return t switch
        {
            UpgradeType.MaxHp => "최대 체력 +20, 즉시 회복",
            UpgradeType.HpRestore => "현재 체력의 30% 즉시 회복",
            UpgradeType.MoveSpeed => "이동속도 +10%",
            UpgradeType.FireRate => "발사 딜레이 -15%",
            UpgradeType.BulletDamage => "총알 데미지 +20%",
            UpgradeType.BulletSpeed => "총알 속도 +15%",
            UpgradeType.Pierce => "총알이 적 1마리 추가 관통",
            UpgradeType.DashCooldown => "대시 쿨다운 -20%",
            _ => ""
        };
    }
}
