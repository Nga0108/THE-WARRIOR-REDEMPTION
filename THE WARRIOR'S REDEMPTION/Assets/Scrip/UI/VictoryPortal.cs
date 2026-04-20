using UnityEngine;

public class VictoryPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu người chơi chạm vào cửa
        if (collision.CompareTag("Player"))
        {
            ItemManager im = collision.GetComponent<ItemManager>();
            if (im != null)
            {
                // Gọi hàm mở Popup chiến thắng
                im.ShowVictoryPopup();
            }
        }
    }
}