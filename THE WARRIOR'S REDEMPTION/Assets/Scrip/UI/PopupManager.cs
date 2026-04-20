using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("Cấu hình Panels")]
    public GameObject victoryPopup;
    public GameObject defeatPopup;
    public GameObject pausePopup; // Thêm dòng này

    [Header("Text hiển thị Victory")]
    public TextMeshProUGUI victoryGoldText;
    public TextMeshProUGUI victoryOreText;

    [Header("Text hiển thị Defeat")]
    public TextMeshProUGUI defeatGoldText;
    public TextMeshProUGUI defeatOreText;

    [Header("Text hiển thị Pause")]
    public TextMeshProUGUI pauseGoldText; // Thêm dòng này để sửa lỗi CS0103
    public TextMeshProUGUI pauseOreText;  // Thêm dòng này để sửa lỗi CS0103

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Tự động ẩn các popup khi vào màn
        if (victoryPopup) victoryPopup.SetActive(false);
        if (defeatPopup) defeatPopup.SetActive(false);
        if (pausePopup) pausePopup.SetActive(false);
    }

    // --- HÀM GỌI KHI TẠM DỪNG ---
    public void ShowPause()
    {
        // Tìm ItemManager trên Player để lấy dữ liệu thực tế
        ItemManager items = FindFirstObjectByType<ItemManager>();
        if (items != null)
        {
            if (pauseGoldText) pauseGoldText.text = items.goldSlot.count.ToString();
            if (pauseOreText) pauseOreText.text = items.oreSlot.count.ToString();
        }

        if (pausePopup) pausePopup.SetActive(true);
        Time.timeScale = 0f; // Dừng game
    }

    // --- HÀM GỌI KHI TIẾP TỤC ---
    public void ResumeButton()
    {
        if (pausePopup) pausePopup.SetActive(false);
        Time.timeScale = 1f; // Chạy lại game
    }

    // --- HÀM GỌI KHI CHIẾN THẮNG ---
    public void ShowVictory(int gold, int ore)
    {
        if (victoryGoldText) victoryGoldText.text = gold.ToString();
        if (victoryOreText) victoryOreText.text = ore.ToString();
        victoryPopup.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- HÀM GỌI KHI THẤT BẠI ---
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

    // --- HÀM QUAY VỀ MÀN HÌNH CHÍNH ---
    public void BackToMainMenu()
    {
        // 1. Phải trả lại thời gian về 1 để Scene Menu không bị đóng băng
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    // --- HÀM THOÁT GAME ---
    public void QuitButton()
    {
        // Lệnh này sẽ đóng ứng dụng khi đã Build ra file .exe hoặc .apk
        Application.Quit();

        // Dòng này giúp bạn kiểm tra nút có hoạt động không khi đang chạy trong Unity Editor
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif

        Debug.Log("Game đang thoát...");
    }

}