using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Chỉ số cơ bản")]
    public float maxHealth = 500f;
    public float currentHealth;
    public float chaseSpeed = 3.5f;
    public float stopDistance = 2f;

    [Header("Tấn công")]
    public Transform attackPoint;
    public float attackRange = 1.2f;
    public float attackDamage = 20f;
    public float specialDamage = 45f;
    public float specialRange = 5f;
    public float attackCooldown = 2.5f;
    private float nextAttackTime;
    private int attackCount = 0;

    [Header("Radar Hình Chữ Nhật")]
    public Vector2 radarSize = new Vector2(15f, 5f);
    public Vector2 radarOffset = new Vector2(0f, 1f);
    public LayerMask playerLayer;

    [Header("Hiệu ứng & UI")]
    public Slider healthSlider;

    [Header("Cài đặt Portal")]
    public GameObject portalPrefab; 
    public Vector3 portalOffset = new Vector3(0, 1.5f, 0);

    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;
    private bool isFacingRight = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.mass = 100f;
            rb.linearDamping = 1f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    void Update()
    {
        if (isDead || anim == null) return;

        Vector2 radarPos = (Vector2)transform.position + radarOffset;
        Collider2D player = Physics2D.OverlapBox(radarPos, radarSize, 0f, playerLayer);

        if (player != null)
        {
            FlipToTarget(player.transform.position.x);
            float dist = Vector2.Distance(transform.position, player.transform.position);

            if (dist > stopDistance)
            {
                Move(player.transform.position.x, chaseSpeed);
            }
            else
            {
                StopMoving();
                if (Time.time >= nextAttackTime)
                {
                    ExecuteAttack();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
        else
        {
            StopMoving();
        }
    }

    void Move(float targetX, float speed)
    {
        float direction = (targetX > transform.position.x) ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        anim.SetFloat("Speed", 1f);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetFloat("Speed", 0f);
    }

    void FlipToTarget(float targetX)
    {
        if ((targetX > transform.position.x && !isFacingRight) || (targetX < transform.position.x && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    void ExecuteAttack()
    {
        attackCount++;
        if (attackCount >= 3)
        {
            anim.SetTrigger("SpecialAttack");
            attackCount = 0;
        }
        else
        {
            anim.SetTrigger("Attack");
        }
    }

    public void TakeDamage(float dmg, PlayerHealth player)
    {
        if (isDead) return;

        currentHealth -= dmg;
        if (healthSlider != null) healthSlider.value = currentHealth;

        if (player != null)
        {
            player.AddRage(8f);
        }

        if (anim != null) anim.SetTrigger("TakeHit");

        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        LootSpawner spawner = GetComponent<LootSpawner>();
        if (spawner != null)
        {
            spawner.SpawnLoot(true);
        }

        anim.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (healthSlider != null) healthSlider.gameObject.SetActive(false);
        Destroy(gameObject, 5f);

        StartCoroutine(SpawnPortalSequence());
    }

    IEnumerator SpawnPortalSequence()
    {
        yield return new WaitForSeconds(2.5f);

        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, transform.position + portalOffset, Quaternion.identity);
            Debug.Log("Cổng dịch chuyển đã xuất hiện!");
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void PerformSpecialDamage()
    {
        Collider2D p = Physics2D.OverlapCircle(transform.position, specialRange, playerLayer);
        if (p != null) p.GetComponent<PlayerHealth>()?.TakeDamage(specialDamage);
    }

    public void EnemyPerformDamage()
    {
        if (attackPoint == null) return;
        Collider2D p = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (p != null) p.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + radarOffset, radarSize);
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}