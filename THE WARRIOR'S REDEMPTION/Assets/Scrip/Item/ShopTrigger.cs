using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [Header("ShopsPopup ")]
    public GameObject shopPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.CompareTag("Player"))
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true); 
                Time.timeScale = 0f;      
            }
        }
    }
}