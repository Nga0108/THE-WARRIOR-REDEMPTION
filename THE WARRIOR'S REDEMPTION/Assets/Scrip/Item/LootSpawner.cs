using UnityEngine;
using System.Collections;

public class LootSpawner : MonoBehaviour
{
    [Header("Prefabs Vật phẩm")]
    public GameObject goldPrefab;
    public GameObject healthPotionPrefab;
    public GameObject orePrefab;

    [Header("Cài đặt số lượng")]
    public int minGold = 3;
    public int maxGold = 8;

    [Header("Tỉ lệ rơi (%)")]
    [Range(0, 100)] public float potionChance = 10f;
    [Range(0, 100)] public float oreChance = 100f;

    [Header("Hiệu ứng Burst")]
    public float delayBetweenGold = 0.05f;

    public void SpawnLoot(bool isBoss)
    {
        // Sử dụng StartCoroutine để chạy hiệu ứng tung tóe
        StartCoroutine(BurstLootRoutine(isBoss));
    }

    private IEnumerator BurstLootRoutine(bool isBoss)
    {
        // 1. Rớt Vàng
        int goldCount = Random.Range(minGold, maxGold + 1);
        for (int i = 0; i < goldCount; i++)
        {
            DropItem(goldPrefab);
            yield return new WaitForSeconds(delayBetweenGold);
        }

        // 2. Rớt Bình máu (Dựa trên potionChance)
        if (Random.Range(0f, 100f) <= potionChance)
        {
            DropItem(healthPotionPrefab);
        }

        // 3. Rớt Khoáng sản (Nếu là Boss)
        if (isBoss)
        {
            if (Random.Range(0f, 100f) <= oreChance)
            {
                // Rớt 1-2 viên quặng cho bõ công đánh Boss
                DropItem(orePrefab);
            }
        }
    }

    void DropItem(GameObject item)
    {
        if (item == null) return;

        GameObject dropped = Instantiate(item, transform.position, Quaternion.identity);

        Rigidbody2D rb = dropped.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // CHỈNH LẠI LỰC: Văng sang trái/phải ngẫu nhiên và LUÔN hướng lên trên (Y dương)
            float forceX = Random.Range(-3f, 3f);
            float forceY = Random.Range(2f, 5f); // Lực dương để tung lên trời

            rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
        }
    }
}