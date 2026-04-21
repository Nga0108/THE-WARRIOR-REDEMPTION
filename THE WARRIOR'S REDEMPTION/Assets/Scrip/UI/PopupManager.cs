using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("Cấu hình Panels")]
    public GameObject victoryPopup;
    public GameObject defeatPopup;
    public GameObject pausePopup;
    public GameObject settingsPopup;
    public GameObject pauseGoldGroup;
    public GameObject pauseOreGroup;

    [Header("Text hiển thị Victory")]
    public TextMeshProUGUI victoryGoldText;
    public TextMeshProUGUI victoryOreText;

    [Header("Text hiển thị Defeat")]
    public TextMeshProUGUI defeatGoldText;
    public TextMeshProUGUI defeatOreText;

    [Header("Text hiển thị Pause")]
    public TextMeshProUGUI pauseGoldText;
    public TextMeshProUGUI pauseOreText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        
        if (victoryPopup) victoryPopup.SetActive(false);
        if (defeatPopup) defeatPopup.SetActive(false);
        if (pausePopup) pausePopup.SetActive(false);
    }

    public void ShowPause()
    {
     
        ItemManager items = FindFirstObjectByType<ItemManager>();

        if (items != null)
        {
         
            if (items.goldSlot.count > 0)
            {
                pauseGoldGroup.SetActive(true);
                if (pauseGoldText) pauseGoldText.text = items.goldSlot.count.ToString();
            }
            else
            {
                pauseGoldGroup.SetActive(false);
            }

         
            if (items.oreSlot.count > 0)
            {
                pauseOreGroup.SetActive(true);
                if (pauseOreText) pauseOreText.text = items.oreSlot.count.ToString();
            }
            else
            {
                pauseOreGroup.SetActive(false);
            }
        }

        if (pausePopup) pausePopup.SetActive(true);
        if (settingsPopup) settingsPopup.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pausePopup) pausePopup.SetActive(false);
        if (settingsPopup) settingsPopup.SetActive(false);

        Time.timeScale = 1f; 
    }

    public void ShowVictory(int gold, int ore)
    {
        if (victoryGoldText) victoryGoldText.text = gold.ToString();
        if (victoryOreText) victoryOreText.text = ore.ToString();
        victoryPopup.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowDefeat(int gold, int ore)
    {
        if (defeatGoldText) defeatGoldText.text = gold.ToString();
        if (defeatOreText) defeatOreText.text = ore.ToString();
        defeatPopup.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevelButton()
    {
        Time.timeScale = 1f;
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
    }

    // Trong PopupManager.cs, tại hàm BackToMainMenu hoặc NextLevelButton

    // Trong PopupManager.cs
    public void BackToMainMenu()
    {
        // Tìm ItemManager để lấy số vàng đang nhặt được trong màn này
        ItemManager items = FindFirstObjectByType<ItemManager>();

        if (items != null)
        {
            // Gọi DataManager để thực hiện cộng dồn
            if (DataManager.Instance != null)
            {
                DataManager.Instance.SaveGame(items.goldSlot.count, items.oreSlot.count);
            }
            else
            {
                // Trường hợp chưa có Instance (nếu bạn không dùng DontDestroyOnLoad)
                // Ta có thể dùng tạm cách này:
                int currentGold = PlayerPrefs.GetInt("SavedGold", 0) + items.goldSlot.count;
                PlayerPrefs.SetInt("SavedGold", currentGold);
                PlayerPrefs.Save();
            }
        }

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Spawn");
    }

    public void OpenSettings()
    {
        if (settingsPopup) settingsPopup.SetActive(true);
        if (pausePopup) pausePopup.SetActive(false);
    }


    public void OpenInventory()
    {
        if (pausePopup) pausePopup.SetActive(true);
        if (settingsPopup) settingsPopup.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPopup != null) settingsPopup.SetActive(false);
    }

  
    public void SetMusicVolume(float value)
    {
      
        Debug.Log("Âm lượng Nhạc: " + value);
    }

    
    public void SetSFXVolume(float value)
    {
        Debug.Log("Âm lượng Hiệu ứng: " + value);
    }

  
    public void QuitButton()
    {
   
        Application.Quit();

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif

        Debug.Log("Game đang thoát...");
    }

}