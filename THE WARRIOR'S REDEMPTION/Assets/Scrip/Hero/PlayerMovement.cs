using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc phải có thư viện này

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float dashForce = 20f;

    private Animator anim;
    private Rigidbody2D rb;
    private float lastDashTime;
    private int jumpCount = 0;
    private bool isGrounded;

    public Transform attackPoint;    // Tạo 1 Empty Object nằm phía trước Player
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Kiểm tra xem bàn phím có đang hoạt động không
        if (Keyboard.current == null) return;

        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleDash();
        HandleAttack();

        // Cập nhật các tham số cho Animator
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    // 1. Xử lý di chuyển và lật hướng (Flip)
    void HandleMovement()
    {
        float move = 0;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move = -1;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move = 1;

        // Không cho phép đổi hướng trong 0.2s đầu của Dash
        if (Time.time - lastDashTime > 0.2f)
        {
            rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
            anim.SetFloat("Speed", Mathf.Abs(move));

            if (move > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (move < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // 2. Nhảy và Nhảy đôi (Double Jump)
    void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && jumpCount < 2)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;

            if (jumpCount == 1)
                anim.SetTrigger("jump");
            else
                anim.SetTrigger("jumptoFall"); // Animation nhảy lần 2

            isGrounded = false;
        }
    }

    // 3. Ngồi (Crouch)
    void HandleCrouch()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame) anim.SetBool("isCrouching", true);
        if (Keyboard.current.sKey.wasReleasedThisFrame) anim.SetBool("isCrouching", false);
    }

    // 4. Dash (Lướt)
    void HandleDash()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            anim.SetTrigger("dash");
            lastDashTime = Time.time;
            float dashDirection = transform.localScale.x;
            rb.linearVelocity = new Vector2(dashDirection * dashForce, rb.linearVelocity.y);
        }
    }

    // 5. Tấn công & Combo Dash-Attack
    void HandleAttack()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            if (Time.time - lastDashTime < 0.3f)
                anim.SetTrigger("dashAttack");
            else
                anim.SetTrigger("attack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.GetComponent<EnemyAI>() != null)
                {
                    enemy.GetComponent<EnemyAI>().TakeDamage(15); // Đánh quái mất 20 máu
                }
            }
        }
    }

    // Kiểm tra va chạm sàn
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void TakeDamage()
    {
        anim.SetTrigger("hurt");
    }
}