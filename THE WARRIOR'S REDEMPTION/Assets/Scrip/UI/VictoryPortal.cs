using UnityEngine;

public class VictoryPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ItemManager im = collision.GetComponent<ItemManager>();
            if (im != null)
            {
                im.ShowVictoryPopup();
            }
        }
    }
}