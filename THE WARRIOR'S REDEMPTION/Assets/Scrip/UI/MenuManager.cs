using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Cài đặt Popup")]
    public GameObject VictoryPopup;
    public GameObject DefeatPopup;

    [Header("Text hiển thị trên bảng Defeat")]
    public TextMeshProUGUI goldLostText;
    public TextMeshProUGUI oreLostText; 


 
    public void ShowDefeatPopup(int gold, int ore)
    {
    
        if (goldLostText != null) goldLostText.text = gold.ToString();
        if (oreLostText != null) oreLostText.text = ore.ToString();

        if (DefeatPopup != null)
        {
            DefeatPopup.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}