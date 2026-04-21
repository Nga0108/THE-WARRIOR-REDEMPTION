using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; 

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI Panels")]
    public GameObject shopPanel;

    [Header("Giá vật phẩm")]
    public int hpPrice = 50;
    public int damagePrice = 100;
    public int defensePrice = 100;

    [Header("Cheat Settings")]
    public int cheatGoldAmount = 1000;

    private ItemManager itemManager;
    private DataManager dataManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (shopPanel) shopPanel.SetActive(false);
    }

    private void Start()
    {
        itemManager = FindFirstObjectByType<ItemManager>();
        dataManager = DataManager.Instance;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            AddCheatGold();
        }

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            ResetGold();
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void CloseShop()
    {
       
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Time.timeScale = 1f; 
        }
    }

    public void BuyHP() => TryBuyItem(hpPrice, "HP");
    public void BuyDamage() => TryBuyItem(damagePrice, "Damage");
    public void BuyDefense() => TryBuyItem(defensePrice, "Defense");

    private void TryBuyItem(int price, string itemType)
    {
        int currentGold = PlayerPrefs.GetInt("SavedGold", 0);

        if (currentGold >= price)
        {
            currentGold -= price;
            PlayerPrefs.SetInt("SavedGold", currentGold);

           
            int currentItemCount = PlayerPrefs.GetInt("Owned_" + itemType, 0);
            PlayerPrefs.SetInt("Owned_" + itemType, currentItemCount + 1);

            PlayerPrefs.Save();

            if (dataManager != null) dataManager.UpdateMainUI();

           
            if (itemManager != null)
            {
                if (itemType == "HP") itemManager.hpSlot.count++;
                else if (itemType == "Damage") itemManager.damageSlot.count++;
                else if (itemType == "Defense") itemManager.defenseSlot.count++;
                itemManager.UpdateInventoryUI();
            }
           
        }
    }

    public void AddCheatGold()
    {
        int currentGold = PlayerPrefs.GetInt("SavedGold", 0);
        currentGold += cheatGoldAmount;
        PlayerPrefs.SetInt("SavedGold", currentGold);
        PlayerPrefs.Save();

        if (dataManager != null) dataManager.UpdateMainUI();
        Debug.Log("Đã hack thêm tiền! Tổng hiện tại: " + currentGold);
    }

    public void ResetGold()
    {
        PlayerPrefs.SetInt("SavedGold", 0);
        PlayerPrefs.Save();

        if (dataManager != null) dataManager.UpdateMainUI();
        Debug.Log("Đã Reset tiền về 0!");
    }

}