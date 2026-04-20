using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Di chuyển & Nhảy")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    private bool isCrouching = false;

    [Header("Cài đặt Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 3f;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Cài đặt tấn công")]
    public Transform attackPoint;
    public float attackRange = 0.6f;
    public float attackDamage = 25f;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;
    [HideInInspector] public bool isAttacking = false;

    [Header("Cài đặt Nộ (Rage Mode)")]
    public float damageMultiplier = 1f;
    private bool isRageActive = false;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerHealth playerHealth;
    private float lastDashTime;
    private int jumpCount = 0;
    private bool isGrounded;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.isDead) return;
        if (Keyboard.current == null) return;

        HandleAttack();
        HandleRageInput();
        HandleCrouch();

        if (isDashing) return;

        bool canMove = true;

        if (playerHealth != null && playerHealth.isHurting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            isAttacking = false;
            canMove = false;
        }

        if (isAttacking && !anim.GetCurrentAnimatorStateInfo(0).IsName("DashAttack"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            canMove = false;
        }

        if (canMove && !isCrouching)
        {
            HandleMovement();
            HandleJump();
            HandleDashInput();
        }
        else if (isCrouching)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0);
        }

        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    // Sửa lỗi Animation Event "ForceFinishHurt"
    public void ForceFinishHurt()
    {
        if (playerHealth != null) playerHealth.isHurting = false;
    }

    void HandleCrouch()
    {
        if (!isGrounded || isDashing)
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
            return;
        }
        isCrouching = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;
        anim.SetBool("isCrouching", isCrouching);
    }

    void HandleMovement()
    {
        float move = 0;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move = -1;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move = 1;

        if (Time.time - lastDashTime > dashDuration)
        {
            rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
            anim.SetFloat("Speed", Mathf.Abs(move));
            if (move > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (move < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void HandleAttack()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            isAttacking = true;
            nextAttackTime = Time.time + attackCooldown;
            anim.SetTrigger(isDashing ? "dashAttack" : "attack");
        }
    }

    void HandleDashInput()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame && canDash)
            StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        lastDashTime = Time.time;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashForce, 0f);
        anim.SetTrigger("dash");
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void HandleRageInput()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame && playerHealth != null)
        {
            if (playerHealth.currentRage >= playerHealth.maxRage && !isRageActive)
                StartCoroutine(ActivateRageMode());
        }
    }

    private IEnumerator ActivateRageMode()
    {
        isRageActive = true;
        playerHealth.currentRage = 0;
        playerHealth.UpdateRageUI();
        playerHealth.isInvincible = true;
        damageMultiplier = 1.5f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 0.4f, 0.4f);
        yield return new WaitForSeconds(3f);
        playerHealth.isInvincible = false;
        yield return new WaitForSeconds(2f);
        damageMultiplier = 1f;
        isRageActive = false;
        if (sr != null) sr.color = Color.white;
    }

    public void PerformDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D target in hitEnemies)
        {
            float finalDamage = attackDamage * damageMultiplier;

            // Kiểm tra nếu là quái thường
            EnemyController enemy = target.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage, this.playerHealth);
                continue; // Đã xử lý xong đối tượng này
            }

            // THÊM ĐOẠN NÀY: Kiểm tra nếu là Boss
            BossController boss = target.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(finalDamage, this.playerHealth);
            }
        }
    }

    public void FinishAttack() { isAttacking = false; }

    void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && jumpCount < 2)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            anim.SetTrigger(jumpCount == 1 ? "jump" : "jumptoFall");
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) { isGrounded = true; jumpCount = 0; }
    }

    // Reset trạng thái khi rời mặt đất để tránh kẹt Jump
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) { isGrounded = false; }
    }
}