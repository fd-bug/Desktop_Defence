using UnityEngine;

public class Enemy_Pathing : MonoBehaviour
{
    [Header("移動設定")]
    public float speed = 2f;

    [Header("路徑節點")]
    public Transform[] waypoints;

    private int wavepointIndex = 0;

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("請在 Inspector 中指派 Waypoints 路徑點！");
            return;
        }
        transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (wavepointIndex >= waypoints.Length) return;

        Transform targetWaypoint = waypoints[wavepointIndex];
        Vector3 dir = targetWaypoint.position - transform.position;

        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (dir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) <= 0.2f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        wavepointIndex++;

        if (wavepointIndex >= waypoints.Length)
        {
            EndPath();
        }
    }

    // 🎯 修改這裡的終點邏輯
    void EndPath()
    {
        // 💔 【全新新增】呼叫玩家 Stats 扣除生命值
        if (Player_Stats.instance != null)
        {
            Player_Stats.instance.TakeDamage(1); // 每隻怪漏掉扣 1 點血
        }
        else
        {
            Debug.LogWarning("找不到 Player_Stats 實例，請確認場景中是否有掛載該腳本！");
        }

        // 原本就有的摧毀敵人邏輯
        Destroy(gameObject);
    }
}