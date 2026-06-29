using UnityEngine;

public class Tower_Aim : MonoBehaviour
{
    [Header("偵測與攻擊設定")]
    public float range = 2f;
    public float fireRate = 1f;
    public float damage = 20f;
    public LayerMask enemyLayer;

    [Header("🎯 投射物發射設定")]
    public GameObject projectilePrefab; // 你的子彈預製物 (例如 Pencil_Proj)
    public Transform firePoint;         // 子彈的發射起點 (例如筆尖 Pencil_red_Firepoint)



    [Header("旋轉轉向設定 (從 Inspector 引入)")]
    public Transform partToRotate;      // 🎯 對應你的 Part To Rotate 欄位
    public float turnSpeed = 10f;       // 🎯 對應你的 Turn Speed 欄位
    private float attackCooldown = 0f;


    void Update()
    {
        // 1. 倒數計時器
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        // 2. 尋找最近的怪物
        Transform targetEnemy = FindNearestEnemy();

        // 🎯 3. 【全新補上】即時旋轉轉向邏輯
        if (targetEnemy != null && partToRotate != null)
        {
            // 計算從防禦塔指向怪物的方向向量
            Vector3 dir = targetEnemy.position - partToRotate.position;

            // 關鍵：因為筆是立在桌上的，我們只鎖定 XZ 平面的旋轉，防止筆身歪斜倒下
            dir.y = 0f;

            // 將方向向量轉換為 Quaternion 角度
            Quaternion lookRotation = Quaternion.LookRotation(dir);

            // 使用 Lerp / Slerp 讓轉動過程變得平滑流暢
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;

            // 將旋轉角度套用回負責轉向的模型物件上
            partToRotate.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }

        // 4. 如果時間到了就發動攻擊（發射子彈）
        if (targetEnemy != null && attackCooldown <= 0f)
        {
            Attack(targetEnemy);
            attackCooldown = 1f / fireRate;
        }
    }

    void Attack(Transform enemyTransform)
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Tower_Proj projectile = projObj.GetComponent<Tower_Proj>();

            if (projectile != null)
            {
                projectile.Setup(enemyTransform, damage);
                Debug.Log($"🎯 [{gameObject.name}] 成功發射子彈追擊 [{enemyTransform.name}]！");
            }
        }
    }

    Transform FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, enemyLayer);
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, col.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = col.transform;
            }
        }
        return nearestEnemy;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void UpgradeTower()
    {
        damage *= 1.2f; // 傷害預設每次 * 1.2
        Debug.Log($"🖊️ [{gameObject.name}] 升級了！目前單體傷害提升至：{damage}");
    }

}