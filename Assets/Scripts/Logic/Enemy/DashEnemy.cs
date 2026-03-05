using UnityEngine;

public class DashEnemy : EnemyBase
{
    enum State { Walking, Telegraphing, Dashing }

    [Header("Dash Settings")]
    [SerializeField] float walkSpeed = 1.2f;
    [SerializeField] float dashSpeed = 14f;
    [SerializeField] float dashDuration = 0.3f;
    [SerializeField] float telegraphDuration = 0.8f;
    [SerializeField] float dashCooldown = 3f;
    [SerializeField] float dashTriggerRange = 5f;

    State _state = State.Walking;
    float _stateTimer;
    float _cooldownTimer;
    Vector2 _dashDirection;
    SpriteRenderer _spriteRenderer;
    Color _originalColor;
    float _blinkTimer;

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
            _originalColor = _spriteRenderer.color;
        _cooldownTimer = dashCooldown;
    }

    void Update()
    {
        if (player == null) return;

        switch (_state)
        {
            case State.Walking:
                UpdateWalking();
                break;
            case State.Telegraphing:
                UpdateTelegraphing();
                break;
            case State.Dashing:
                UpdateDashing();
                break;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        switch (_state)
        {
            case State.Walking:
                Vector2 dir = ((Vector2)player.position - rb.position).normalized;
                rb.linearVelocity = dir * walkSpeed;
                rb.SetRotation(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);
                break;
            case State.Telegraphing:
                rb.linearVelocity = Vector2.zero;
                break;
            case State.Dashing:
                rb.linearVelocity = _dashDirection * dashSpeed;
                break;
        }
    }

    void UpdateWalking()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (_cooldownTimer <= 0f)
        {
            float dist = Vector2.Distance(rb.position, player.position);
            if (dist <= dashTriggerRange)
            {
                _state = State.Telegraphing;
                _stateTimer = telegraphDuration;
                _dashDirection = ((Vector2)player.position - rb.position).normalized;
                _blinkTimer = 0f;
            }
        }
    }

    void UpdateTelegraphing()
    {
        _stateTimer -= Time.deltaTime;

        // Blink red
        if (_spriteRenderer != null)
        {
            _blinkTimer += Time.deltaTime;
            bool showRed = Mathf.Sin(_blinkTimer * 15f) > 0f;
            _spriteRenderer.color = showRed ? Color.red : _originalColor;
        }

        if (_stateTimer <= 0f)
        {
            _state = State.Dashing;
            _stateTimer = dashDuration;
            if (_spriteRenderer != null)
                _spriteRenderer.color = _originalColor;
        }
    }

    void UpdateDashing()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f)
        {
            _state = State.Walking;
            _cooldownTimer = dashCooldown;
        }
    }

    protected override void Die()
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = _originalColor;
        base.Die();
    }
}
