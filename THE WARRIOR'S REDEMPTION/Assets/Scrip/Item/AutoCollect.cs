using UnityEngine;

public class AutoCollect : MonoBehaviour
{
    [Header("Cài đặt hút")]
    public float collectDistance = 5f;
    public float moveSpeed = 10f;
    public float delayBeforeFly = 0.6f; // Chờ rơi xong mới bay

    private Transform player;
    private bool canFly = false;
    private Rigidbody2D rb;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        rb = GetComponent<Rigidbody2D>();
        Invoke("EnableFlying", delayBeforeFly);
    }

    void EnableFlying() => canFly = true;

    void Update()
    {
        if (player == null || !canFly) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= collectDistance)
        {
            if (rb != null)
            {
                // Quan trọng: Phải tắt hoàn toàn vật lý khi bắt đầu hút
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
                rb.simulated = true; // Vẫn giữ simulated để nhận Trigger nhặt đồ
            }

            moveSpeed += Time.deltaTime * 8f;
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ItemManager im = collision.GetComponent<ItemManager>();
            if (im != null)
            {
                // Chuyển tên về chữ thường để so sánh không bị sai chữ hoa/thường
                string name = gameObject.name.ToLower();

                // KIỂM TRA BÌNH MÁU
                // Hãy xem tên Prefab của bạn là gì (trong hình là Health_Potion...)
                if (name.Contains("health"))
                {
                    im.hpSlot.count++; // Tăng số lượng trong slot máu
                    Debug.Log("Đã nhặt bình máu! Số lượng hiện tại: " + im.hpSlot.count);
                }
                else if (name.Contains("gold"))
                {
                    im.AddGold(1);
                }
                else if (name.Contains("ore"))
                {
                    im.AddOre(1);
                }

                // QUAN TRỌNG: Cập nhật lại giao diện sau khi nhặt
                im.UpdateInventoryUI();

                Destroy(gameObject);
            }
        }
    }
}