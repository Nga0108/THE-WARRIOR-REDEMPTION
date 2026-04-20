using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Thư viện bắt buộc để chuyển cảnh

public class MenuManager : MonoBehaviour
{
    [Header("Cài đặt Popup")]
    public GameObject VictoryPopup;
    public GameObject DefeatPopup;

    [Header("Text hiển thị trên bảng Defeat")]
    public TextMeshProUGUI goldLostText; // Text hiện vàng
    public TextMeshProUGUI oreLostText;  // Text hiện khoáng sản


    // Hàm hiện bảng thua cuộc kèm dữ liệu
    public void ShowDefeatPopup(int gold, int ore)
    {
        // Gán con số nhặt được vào Text trước khi hiện bảng
        if (goldLostText != null) goldLostText.text = gold.ToString();
        if (oreLostText != null) oreLostText.text = ore.ToString();

        // Hiện bảng và dừng game
        if (DefeatPopup != null)
        {
            DefeatPopup.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // 1. Hàm Chơi Lại (Restart)
    public void RestartGame()
    {
        // Reset lại thời gian (nếu game đang bị pause)
        Time.timeScale = 1f;

        // Lấy tên cảnh hiện tại và load lại
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // 2. Hàm Tiếp Tục (Next Level)
    public void NextLevel()
    {
        // Reset lại thời gian
        Time.timeScale = 1f;

        // Lấy chỉ số (index) của cảnh tiếp theo trong Build Settings
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Kiểm tra xem có cảnh tiếp theo không
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Bạn đã hoàn thành tất cả các màn chơi!");
            // Bạn có thể cho quay về Menu chính: SceneManager.LoadScene(0);
        }
    }

    // 3. Hàm Thoát Game (Dành cho nút Quit nếu cần)
    public void QuitGame()
    {
        Debug.Log("Thoát game...");
        Application.Quit();
    }
}