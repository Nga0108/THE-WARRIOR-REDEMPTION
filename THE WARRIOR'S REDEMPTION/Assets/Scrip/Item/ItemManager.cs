using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class ItemManager : MonoBehaviour
{
    [System.Serializable]
    public class ItemSlot
    {
        public int count;
        public TextMeshProUGUI countText;
        public GameObject badgeObject;
    }

    [Header("Cấu hình vật phẩm tiêu thụ")]
    public ItemSlot hpSlot;
    public ItemSlot damageSlot;
    public ItemSlot defenseSlot;

    [Header("Cấu hình tài nguyên")]
    public ItemSlot goldSlot;
    public ItemSlot oreSlot;

    [Header("Cài đặt hiệu ứng")]
    public float damageBoostMultiplier = 2f;
    public float boostDuration = 5f;

    [Header("Victory Popup UI")]
    public GameObject victoryPopup;        // Kéo bảng VictoryPopup vào đây
    public TextMeshProUGUI goldResultText; // Text hiện số vàng TRONG POPUP
    public TextMeshProUGUI oreResultText;  // Text hiện số ore TRONG POPUP

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        UpdateInventoryUI();
    }

    void Update()
    {
        if (Keyboard.current == null || (playerHealth != null && playerHealth.isDead)) return;

        if (Keyboard.current.digit4Key.wasPressedThisFrame) UseHP();
        if (Keyboard.current.digit5Key.wasPressedThisFrame) UseDamage();
        if (Keyboard.current.digit6Key.wasPressedThisFrame) UseDefense();
    }

    // --- HÀM CỘNG VẬT PHẨM (Gọi từ AutoCollect) ---
    public void AddGold(int amount)
    {
        goldSlot.count += amount;
        // Thường vàng/ore ẩn nên không cần UpdateInventoryUI ở đây nếu chỉ hiện ở Popup
    }

    public void AddOre(int amount)
    {
        oreSlot.count += amount;
    }

    public void AddHP(int amount)
    {
        hpSlot.count += amount;
        UpdateInventoryUI(); // Cập nhật ngay lên HUD
    }

    // --- HÀM XỬ LÝ POPUP ---
    public void ShowVictoryPopup()
    {
        if (PopupManager.Instance != null)
        {
            // Gửi dữ liệu vàng và ore sang Manager để hiển thị lên bảng Victory
            PopupManager.Instance.ShowVictory(goldSlot.count, oreSlot.count);
        }
    }

    public void CloseVictoryPopup()
    {
        Time.timeScale = 1f;
        if (victoryPopup != null) victoryPopup.SetActive(false);
    }

    // --- HÀM SỬ DỤNG VẬT PHẨM ---
    public void UseHP()
    {
        if (hpSlot.count > 0 && playerHealth.currentHealth < playerHealth.maxHealth)
        {
            hpSlot.count--;
            playerHealth.currentHealth = Mathf.Min(playerHealth.currentHealth + 30f, playerHealth.maxHealth);
            playerHealth.UpdateHealthUI();
            UpdateInventoryUI();
        }
    }

    public void UseDamage() { if (damageSlot.count > 0) { damageSlot.count--; UpdateInventoryUI(); StartCoroutine(DamageBoostRoutine()); } }
    public void UseDefense() { if (defenseSlot.count > 0) { defenseSlot.count--; UpdateInventoryUI(); StartCoroutine(DefenseBoostRoutine()); } }

    private IEnumerator DamageBoostRoutine()
    {
        float originalMultiplier = playerMovement.damageMultiplier;
        playerMovement.damageMultiplier *= damageBoostMultiplier;
        yield return new WaitForSeconds(boostDuration);
        playerMovement.damageMultiplier = originalMultiplier;
    }

    private IEnumerator DefenseBoostRoutine()
    {
        playerHealth.damageReduction = 0.6f;
        yield return new WaitForSeconds(boostDuration);
        playerHealth.damageReduction = 1f;
    }

    public void UpdateInventoryUI()
    {
        UpdateSlotDisplay(hpSlot);
        UpdateSlotDisplay(damageSlot);
        UpdateSlotDisplay(defenseSlot);
        UpdateSlotDisplay(goldSlot);
        UpdateSlotDisplay(oreSlot);
    }

    private void UpdateSlotDisplay(ItemSlot slot)
    {
        if (slot == null) return;
        if (slot.countText != null) slot.countText.text = slot.count.ToString();
        if (slot.badgeObject != null) slot.badgeObject.SetActive(slot.count > 0);
    }
}