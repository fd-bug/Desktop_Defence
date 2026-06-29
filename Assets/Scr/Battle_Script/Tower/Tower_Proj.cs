using UnityEngine;

public class Tower_Proj : MonoBehaviour
{
    private Transform target;      // 子彈要追蹤的目標
    private float damage;          // 由防禦塔傳承過來的傷害值

    [Header("子彈動態設定")]
    public float speed = 5f;       // 子彈飛行的速度
    public float hitDistance = 0.1f; // 判定撞擊怪物的距離

    // 🎯 提供給防禦塔呼叫的初始化方法
    public void Setup(Transform _target, float _damage)
    {
        target = _target;
        damage = _damage;
    }

    void Update()
    {
        // 🛑 防呆：如果飛到一半怪物已經先被別的塔打死了，子彈失去目標就自行自我摧毀
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 🚀 追蹤邏輯：計算往怪物移動的方向
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // 檢查這一格的移動距離是否已經足以撞到怪物
        if (dir.magnitude <= distanceThisFrame || Vector3.Distance(transform.position, target.position) <= hitDistance)
        {
            HitTarget();
            return;
        }

        // 朝目標前進並旋轉面向目標
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    // 💥 當子彈成功咬到怪物時觸發
    void HitTarget()
    {
        if (target != null)
        {
            Enemy_Stats enemy = target.GetComponent<Enemy_Stats>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // 真正造成傷害的時間點改到這裡！
            }
        }

        // 💡 未來可以在這裡實作：生成爆炸特效（Instantiate Particle）

        // 銷毀子彈本身
        Destroy(gameObject);
    }
}