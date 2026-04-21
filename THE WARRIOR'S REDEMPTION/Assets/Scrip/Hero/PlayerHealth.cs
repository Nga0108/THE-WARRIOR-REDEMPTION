using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Chỉ số máu")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;

    [Header("Hệ thống Nộ (Rage)")]
    public Image rageBarFill;
    public float currentRage = 0f;
    public float maxRage = 100f;

    [Header("Hiệu ứng Phát sáng")]
    public float glowSpeed = 5f;
    [ColorUsage(true, true)] public Color glowColor = Color.white;
    private Color originalRageColor;

    [Header("Trạng thái")]
    public bool isDead = false;
    public bool isHurting = false;
    public bool isInvincible = false;
    [Tooltip("1 = Nhận 100%, 0.6 = Nhận 60% (Giảm 40%)")]
    public float damageReduction = 1f;

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (rageBarFill != null) originalRageColor = rageBarFill.color;

        UpdateHealthUI();
        UpdateRageUI();
    }

    void Update()
    {
        HandleRageGlow();
    }

    public void AddRage(float amount)
    {
        if (isDead) return;
        currentRage = Mathf.Clamp(currentRage + amount, 0, maxRage);
        UpdateRageUI();
    }

    public void UpdateRageUI()
    {
        if (rageBarFill != null)
            rageBarFill.fillAmount = currentRage / maxRage;
    }

    private void HandleRageGlow()
    {
        if (rageBarFill == null) return;
        if (currentRage >= maxRage)
        {
            float emission = 0.5f + Mathf.PingPong(Time.time * glowSpeed, 1.0f);
            rageBarFill.color = glowColor * emission;
            rageBarFill.rectTransform.localScale = Vector3.one * (1f + (emission - 0.5f) * 0.08f);
        }
        else
        {
            rageBarFill.color = originalRageColor;
            rageBarFill.rectTransform.localScale = Vector3.one;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isHurting || isInvincible) return;

        // Áp dụng giảm sát thương 40%
        float finalDamage = damage * damageReduction;

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0) PlayerDie();
        else StartCoroutine(HurtingRoutine());
    }

    public void UpdateHealthUI()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    void PlayerDie()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("Die");

        ItemManager items = GetComponent<ItemManager>();
        if (items != null && PopupManager.Instance != null)
        {
        
            PopupManager.Instance.ShowDefeat(items.goldSlot.count, items.oreSlot.count);
        }

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<PlayerMovement>().enabled = false;
    }

    private IEnumerator HurtingRoutine()
    {
        isHurting = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (anim != null)
        {
            anim.ResetTrigger("hurt");
            anim.SetTrigger("hurt");
        }

        yield return new WaitForSeconds(0.2f);
        isHurting = false;
        if (anim != null) anim.ResetTrigger("hurt");
    }
}