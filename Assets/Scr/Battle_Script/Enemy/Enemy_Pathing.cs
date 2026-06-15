using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [Header("移動設定")]
    public float speed = 2f;

    [Header("路徑節點")]
    public Transform[] waypoints;

    private int wavepointIndex = 0;

    void Start()
    {
        // 確保一開始有取得路徑節點
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("請在 Inspector 中指派 Waypoints 路徑點！");
            return;
        }

        // 將敵人初始位置定位到第一個節點
        transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (wavepointIndex >= waypoints.Length) return;

        // 朝向下一個節點前進
        Transform targetWaypoint = waypoints[wavepointIndex];
        Vector3 dir = targetWaypoint.position - transform.position;

        // 移動物體（只在 XZ 平面移動，保持高度穩定）
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        // 面向移動方向（讓紙張前進時看起來有方向感）
        if (dir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        // 檢查是否接近當前節點，接近了就換下一個
        if (Vector3.Distance(transform.position, targetWaypoint.position) <= 0.2f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        wavepointIndex++;

        // 到達終點（電腦前）
        if (wavepointIndex >= waypoints.Length)
        {
            EndPath();
        }
    }

    void EndPath()
    {
        // 暫時直接摧毀敵人，後續可以扣除玩家生命值
        Destroy(gameObject);
    }
}