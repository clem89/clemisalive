using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Shooting")]
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 0.2f;

    [Header("Stats")]
    [SerializeField] int maxHealth = 100;

    int _currentHealth;
    Vector2 _moveInput;
    float _nextFireTime;
    float _dashEndTime;
    float _dashCooldownEnd;
    bool _isDashing;
    Vector2 _dashDirection;
    Rigidbody2D _rb;
    Camera _camera;

    // Runtime stats
    float _rtMoveSpeed;
    float _rtDashCooldown;
    float _rtFireRate;
    int _rtBulletDamage = 20;
    float _rtBulletSpeed = 12f;
    int _rtPierceCount = 0;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        _currentHealth = maxHealth;
        _rtMoveSpeed = moveSpeed;
        _rtDashCooldown = dashCooldown;
        _rtFireRate = fireRate;
    }

    void Update()
    {
        ReadMoveInput();
        AimAtMouse();
        HandleShoot();
        HandleDash();
    }

    void FixedUpdate()
    {
        if (_isDashing)
        {
            if (Time.time >= _dashEndTime) _isDashing = false;
            else _rb.linearVelocity = _dashDirection * dashSpeed;
        }
        else
        {
            _rb.linearVelocity = _moveInput * _rtMoveSpeed;
        }
    }

    void ReadMoveInput()
    {
        var kb = Keyboard.current;
        float x = 0f, y = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y += 1f;
        _moveInput = new Vector2(x, y).normalized;
    }

    void AimAtMouse()
    {
        Vector2 mouseWorld = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        if (dir != Vector2.zero)
            transform.up = dir;
    }

    void HandleShoot()
    {
        if (Mouse.current.leftButton.isPressed) TryShoot();
    }

    void TryShoot()
    {
        if (Time.time < _nextFireTime) return;
        _nextFireTime = Time.time + _rtFireRate;

        Transform spawnPoint = firePoint != null ? firePoint : transform;
        BulletPool.Instance.Get(spawnPoint.position, spawnPoint.rotation, _rtBulletDamage, _rtBulletSpeed, _rtPierceCount);
    }

    void HandleDash()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        if (Time.time < _dashCooldownEnd || _isDashing) return;

        _isDashing = true;
        _dashEndTime = Time.time + dashDuration;
        _dashCooldownEnd = Time.time + _rtDashCooldown;
        _dashDirection = _moveInput.sqrMagnitude > 0
            ? _moveInput.normalized
            : (Vector2)transform.up;
    }

    public void TakeDamage(int amount)
    {
        if (_isDashing) return;

        _currentHealth -= amount;
        if (_currentHealth <= 0) Die();
    }

    public void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.MaxHp:
                maxHealth += 20;
                _currentHealth = Mathf.Min(_currentHealth + 20, maxHealth);
                break;
            case UpgradeType.HpRestore:
                _currentHealth = Mathf.Min(_currentHealth + Mathf.RoundToInt(maxHealth * 0.3f), maxHealth);
                break;
            case UpgradeType.MoveSpeed:
                _rtMoveSpeed *= 1.1f;
                break;
            case UpgradeType.FireRate:
                _rtFireRate *= 0.85f;
                break;
            case UpgradeType.BulletDamage:
                _rtBulletDamage = Mathf.RoundToInt(_rtBulletDamage * 1.2f);
                break;
            case UpgradeType.BulletSpeed:
                _rtBulletSpeed *= 1.15f;
                break;
            case UpgradeType.Pierce:
                _rtPierceCount++;
                break;
            case UpgradeType.DashCooldown:
                _rtDashCooldown *= 0.8f;
                break;
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
        GameManager.Instance.OnPlayerDied();
    }
}
