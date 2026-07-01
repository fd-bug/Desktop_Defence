using System.Collections.Generic;
using UnityEngine;

public class Plant_Tower : MonoBehaviour
{
    [Header("AOE 屬性設定")]
    public float range = 1.5f;
    public float fireRate = 0.5f;
    public float damage = 10f;

    [Header("緩速效果設定")]
    [Range(0.1f, 0.9f)]
    public float slowFactor = 0.5f;
    public float slowDuration = 2.5f;

    public LayerMask enemyLayer;

    // 🎯 【全新新增】特效配置欄位
    [Header("✨ 視覺特效設定")]
    public GameObject shockwavePrefab;  // 放入你挑選的震波 Prefab
    public float effectDestroyTime = 2f; // 特效自動銷毀的時間（例如選 2s 的特效就填 2）

    private float attackCooldown = 0f;

    void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (attackCooldown <= 0f && IsAnyEnemyInRange())
        {
            ExplodeAOE();
            attackCooldown = 1f / fireRate;
        }
    }

    bool IsAnyEnemyInRange()
    {
        return Physics.CheckSphere(transform.position, range, enemyLayer);
    }

    void ExplodeAOE()
    {
        Debug.Log($"🌵 [{gameObject.name}] 釋放了群體藤蔓震擊！");

        if (shockwavePrefab != null)
        {
            GameObject vfxInstance = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
            Destroy(vfxInstance, effectDestroyTime);
        }

        // 🎯 1. 建立一個點名簿，用來記錄這一發爆炸已經炸過哪些怪物了
        HashSet<Enemy_Stats> AddressedEnemies = new HashSet<Enemy_Stats>();

        // 2. 抓取範圍內的所有碰撞體
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, enemyLayer);

        foreach (Collider col in colliders)
        {
            Enemy_Stats enemy = col.GetComponent<Enemy_Stats>();

            // 🎯 如果沒抓到，嘗試往父物件找（防範 Collider 掛在子模型上的情況）
            if (enemy == null)
            {
                enemy = col.GetComponentInParent<Enemy_Stats>();
            }

            if (enemy != null)
            {
                // 🎯 3. 核心防禦：檢查這隻怪是不是已經受過傷了？
                // Contains 如果回傳 true，代表點名簿裡有了，直接跳過這一個 Collider！
                if (AddressedEnemies.Contains(enemy))
                    continue;

                // 🎯 4. 如果是第一次遇到這隻怪，加入點名簿，並給予傷害與緩速
                AddressedEnemies.Add(enemy);

                enemy.TakeDamage(damage);
                enemy.ApplySlow(slowFactor, slowDuration);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void UpgradeTower()
    {
        damage *= 1.2f; // 傷害預設每次 * 1.2
        Debug.Log($"🌵 [{gameObject.name}] 升級了！目前範圍傷害提升至：{damage}");
    }

}