using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Cài đặt Boss")]
    public GameObject bossObject; // Kéo Boss từ Hierarchy vào đây

    [Header("Quản lý quái")]
    public List<GameObject> enemies = new List<GameObject>();
    private bool bossSpawned = false;

    void Start()
    {
        // Tìm tất cả quái vật nhỏ
        GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemies.AddRange(foundEnemies);

        Debug.Log("<color=cyan>LevelManager:</color> Tổng số quái tìm thấy: " + enemies.Count);

        // Đảm bảo Boss ẩn khi bắt đầu
        if (bossObject != null)
        {
            bossObject.SetActive(false);
        }

        // Kiểm tra danh sách quái mỗi 0.5 giây (tối ưu hơn Update)
        InvokeRepeating("CheckEnemies", 1f, 0.5f);
    }

    void CheckEnemies()
    {
        if (bossSpawned) return;

        // Xóa những quái đã bị Destroy (null) khỏi danh sách
        enemies.RemoveAll(item => item == null);

        // Nếu danh sách trống, kích hoạt Boss
        if (enemies.Count <= 0)
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        if (bossSpawned) return;

        bossSpawned = true;
        CancelInvoke("CheckEnemies"); // Dừng quét sau khi boss đã ra

        if (bossObject != null)
        {
            bossObject.SetActive(true);
            Debug.Log("<color=green>LevelManager:</color> Đã diệt hết quái! BOSS XUẤT HIỆN!");
        }
        else
        {
            Debug.LogError("LỖI: Chưa kéo Boss Object vào LevelManager trong Inspector!");
        }
    }
}