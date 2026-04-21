using UnityEngine;

public class CheatManager : MonoBehaviour
{
    public int goldToGive = 1000;

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddGold(goldToGive);
        }

        
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerPrefs.SetInt("SavedGold", 0);
            PlayerPrefs.Save();
            RefreshUI();
            Debug.Log("Đã reset vàng về 0");
        }
    }

    public void AddGold(int amount)
    {
        int currentGold = PlayerPrefs.GetInt("SavedGold", 0);
        currentGold += amount;

        PlayerPrefs.SetInt("SavedGold", currentGold);
        PlayerPrefs.Save();

        RefreshUI();
        Debug.Log("Đã thêm vàng! Tổng hiện tại: " + currentGold);
    }

    private void RefreshUI()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.UpdateMainUI();
        }
    }
}