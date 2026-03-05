using UnityEngine;

public class Enemy : EnemyBase
{
    [Header("Chaser Settings")]
    [SerializeField] float moveSpeed = 2.5f;

    void FixedUpdate()
    {
        if (player == null) return;
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
        rb.SetRotation(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);
    }
}
