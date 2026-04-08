using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Thông số máu")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Cấu hình UI")]
    public Slider healthSlider;
    public float lerpSpeed = 15f;

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            // Tắt Whole Numbers để thanh máu trượt mượt
            healthSlider.wholeNumbers = false;
        }
    }

    void Update()
    {
        if (isDead) return;

        // Cập nhật thanh máu trượt mượt theo thời gian
        if (healthSlider != null && !Mathf.Approximately(healthSlider.value, currentHealth))
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, lerpSpeed * Time.deltaTime);
        }

        // TEST: Nhấn phím T để tự trừ máu (Sử dụng Input System mới)
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Đảm bảo quái của bạn có Tag là "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10); // Bị quái chạm vào mất 10 máu
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player Die!");

        // 1. Chạy Animation chết
        if (anim != null)
        {
            anim.SetTrigger("Die"); // Tên "Die" phải khớp với Animator của bạn
        }

        // 2. Ép thanh máu về 0 ngay lập tức, không chờ Lerp
        if (healthSlider != null) healthSlider.value = 0;

        // 3. Vô hiệu hóa di chuyển để xác không chạy được
        if (GetComponent<PlayerMovement>() != null)
        {
            GetComponent<PlayerMovement>().enabled = false;
        }

        // 4. Dừng vật lý để không bị đẩy bởi quái
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            // rb.simulated = false; // Mở dòng này nếu muốn xác xuyên thấu
        }
    }
}