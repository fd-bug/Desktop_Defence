using UnityEngine;
namespace fdbug
{
    public class Tower_Proj : MonoBehaviour
    {
        [Header("移動設定")]
        public float speed = 10f; // 飛行的速度

        private Transform target;

        // 路由：由防禦塔呼叫並傳入目標
        public void Seek(Transform _target)
        {
            target = _target;
        }

        void Update()
        {
            // 如果目標在飛行途中已經不見了（例如被其他塔打死、走到終點消失）
            if (target == null)
            {
                Destroy(gameObject); // 投射物自我摧毀
                return;
            }

            // 計算朝向目標的方向與距離
            Vector3 dir = target.position - transform.position;
            float distanceThisFrame = speed * Time.deltaTime;

            // 如果這一幀移動的距離大於等於跟目標的剩餘距離，代表撞擊到了
            if (dir.magnitude <= distanceThisFrame)
            {
                HitTarget();
                return;
            }

            // 正常移動
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);

            // 讓投射物面向目標（可選，適合有方向性的子彈）
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        void HitTarget()
        {
            // 暫時不填入傷害要素，撞到就直接銷毀投射物
            Debug.Log("擊中敵人！");

            // 後續可以在這裡生成爆炸特效 (Instantiate Particle Effect)

            Destroy(gameObject);
        }
    }
}