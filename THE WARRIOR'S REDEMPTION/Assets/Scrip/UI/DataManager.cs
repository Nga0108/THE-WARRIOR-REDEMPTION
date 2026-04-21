using UnityEngine;
using TMPro;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [Header("UI Hiển thị ở Scene Main")]
    public TextMeshProUGUI mainGoldText;
    public TextMeshProUGUI mainOreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateMainUI();
    }

    
    public void SaveGame(int goldEarned, int oreEarned)
    {
        int totalGold = PlayerPrefs.GetInt("SavedGold", 0);
        int totalOre = PlayerPrefs.GetInt("SavedOre", 0);

        
        totalGold += goldEarned;
        totalOre += oreEarned;

       
        PlayerPrefs.SetInt("SavedGold", totalGold);
        PlayerPrefs.SetInt("SavedOre", totalOre);
        PlayerPrefs.Save(); 

        UpdateMainUI();
    }

    public int GetSavedGold() => PlayerPrefs.GetInt("SavedGold", 0);
    public int GetSavedOre() => PlayerPrefs.GetInt("SavedOre", 0);
    public void UpdateMainUI()
    {
        if (mainGoldText != null)
            mainGoldText.text = GetSavedGold().ToString();

        if (mainOreText != null)
            mainOreText.text = GetSavedOre().ToString();
    }
}