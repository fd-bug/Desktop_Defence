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

        // 🎯 1. 【全新新增】在盆栽塔的腳底下生成震波特效
        if (shockwavePrefab != null)
        {
            // 讓特效生成在盆栽的中心點，並保持預設的角度
            GameObject vfxInstance = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);

            // 🎯 2. 【全新新增】設定時間到自動摧毀特效物件，防止場上殘留垃圾物件導致遊戲變卡
            Destroy(vfxInstance, effectDestroyTime);
        }

        // 2. 原本的群體傷害與緩速迴圈（維持不變）
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, enemyLayer);
        foreach (Collider col in colliders)
        {
            Enemy_Stats enemy = col.GetComponent<Enemy_Stats>();
            if (enemy != null)
            {
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