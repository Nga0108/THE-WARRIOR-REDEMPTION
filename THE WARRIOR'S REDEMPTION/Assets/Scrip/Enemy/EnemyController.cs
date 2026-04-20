using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Chỉ số máu")]
    public float maxHealth = 60f;
    public float currentHealth;

    [Header("Di chuyển")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.5f;
    public float stopDistance = 1.0f;
    public float patrolDistance = 4f;

    [Header("Tấn công")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    private float nextAttackTime = 0f;

    [Header("Hệ thống Radar")]
    public Vector2 radarSize = new Vector2(6f, 3f);
    public LayerMask playerLayer;

    private float leftBoundary, rightBoundary;
    private bool isFacingRight = true;
    private bool isDead = false;
    private bool isTakingHit = false;

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        leftBoundary = transform.position.x - patrolDistance;
        rightBoundary = transform.position.x + patrolDistance;

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.mass = 100f;
        }
    }

    void Update()
    {
        if (isDead || isTakingHit) return;

        Collider2D playerCollider = Physics2D.OverlapBox(transform.position, radarSize, 0f, playerLayer);

        if (playerCollider != null)
        {
            LookAtTarget(playerCollider.transform);
            HandleChasing(playerCollider.transform);
        }
        else
        {
            HandlePatrol();
        }

        if (anim != null && rb != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    void LookAtTarget(Transform target)
    {
        if (target.position.x > transform.position.x && !isFacingRight) Flip();
        else if (target.position.x < transform.position.x && isFacingRight) Flip();
    }

    void HandleChasing(Transform player)
    {
        float distanceX = Mathf.Abs(transform.position.x - player.position.x);

        if (distanceX > stopDistance)
        {
            float direction = (player.position.x > transform.position.x) ? 1 : -1;
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (Time.time >= nextAttackTime)
            {
                if (anim != null) anim.SetTrigger("Attack");
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void HandlePatrol()
    {
        rb.linearVelocity = new Vector2(isFacingRight ? patrolSpeed : -patrolSpeed, rb.linearVelocity.y);

        if (isFacingRight && transform.position.x >= rightBoundary) Flip();
        else if (!isFacingRight && transform.position.x <= leftBoundary) Flip();
    }

    // --- SỬA LỖI CS1501 TẠI ĐÂY ---
    // Nhận 2 tham số: sát thương và tham chiếu tới người chơi
    public void TakeDamage(float damage, PlayerHealth player)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Cộng nộ khi đánh trúng (ví dụ: 5 điểm)
        if (player != null)
        {
            player.AddRage(5f);
        }

        if (anim != null) anim.SetTrigger("TakeHit");

        if (currentHealth <= 0)
        {
            Die(player); // Truyền player vào hàm chết để cộng thêm nộ
        }
        else
        {
            StartCoroutine(HitStunRoutine(0.3f));
        }
    }

    IEnumerator HitStunRoutine(float duration)
    {
        isTakingHit = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        isTakingHit = false;
    }

    void Die(PlayerHealth player)
    {
        if (isDead) return;
        isDead = true;

        // Cộng nộ khi tiêu diệt (ví dụ: cộng thêm 15 điểm nữa)
        if (player != null)
        {
            player.AddRage(10f);
        }

        LootSpawner spawner = GetComponent<LootSpawner>();
        if (spawner != null)
        {
            spawner.SpawnLoot(false);
        }

        if (anim != null) anim.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        rb.simulated = false;
        Destroy(gameObject, 2f);
    }

    public void EnemyPerformDamage()
    {
        if (attackPoint == null) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            PlayerHealth pHealth = hitPlayer.GetComponent<PlayerHealth>();
            if (pHealth != null) pHealth.TakeDamage(attackDamage);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, radarSize);

        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}