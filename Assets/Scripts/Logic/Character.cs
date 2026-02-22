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

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        _currentHealth = maxHealth;
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
            _rb.linearVelocity = _moveInput * moveSpeed;
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
        _nextFireTime = Time.time + fireRate;

        Transform spawnPoint = firePoint != null ? firePoint : transform;
        BulletPool.Instance.Get(spawnPoint.position, spawnPoint.rotation);
    }

    void HandleDash()
    {
        // wasPressedThisFrame: 누른 순간 단 1프레임만 true → 대시가 1번만 발동됨
        // isPressed 였다면 꾹 누르는 동안 쿨다운마다 재발동돼버림
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        if (Time.time < _dashCooldownEnd || _isDashing) return;

        _isDashing = true;
        _dashEndTime = Time.time + dashDuration;
        _dashCooldownEnd = Time.time + dashCooldown;
        // sqrMagnitude: 크기의 제곱(√ 연산 없음) → 0인지 아닌지만 판별할 때 magnitude보다 빠름
        _dashDirection = _moveInput.sqrMagnitude > 0
            ? _moveInput.normalized
            : (Vector2)transform.up;
    }

    public void TakeDamage(int amount)
    {
        if (_isDashing) return; // 대시 중 무적

        _currentHealth -= amount;
        if (_currentHealth <= 0) Die();
    }

    void Die()
    {
        gameObject.SetActive(false);
        GameManager.Instance.OnPlayerDied();
    }
}
