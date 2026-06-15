using UnityEngine;

namespace fdbug
{
    public class Tower_Aim : MonoBehaviour
    {
        [Header("偵測與攻擊設定")]
        public float range = 5f;
        public float fireRate = 1f;
        private float fireCountdown = 0f;

        [Header("發射設定 (新加入)")]
        public GameObject projectilePrefab; // 你的投射物 Prefab
        public Transform firePoint;         // 發射位置（例如筆尖的位置）

        [Header("旋轉控制")]
        public Transform partToRotate;
        public float turnSpeed = 10f;

        [Header("目標標籤")]
        public string enemyTag = "Enemy";

        private Transform target;

        void Start()
        {
            InvokeRepeating("UpdateTarget", 0f, 0.5f);
        }

        void UpdateTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null && shortestDistance <= range)
            {
                target = nearestEnemy.transform;
            }
            else
            {
                target = null;
            }
        }

        void Update()
        {
            if (target == null) return;

            LockOnTarget();

            if (fireCountdown <= 0f)
            {
                Attack();
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;
        }

        void LockOnTarget()
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0;

            if (dir != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                Vector3 rotation = Quaternion.Slerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
                partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }
        }

        void Attack()
        {
            // 核心發射邏輯
            if (projectilePrefab != null && firePoint != null)
            {
                // 在發射點生成投射物，角度與發射點一致
                GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

                // 取得投射物身上的腳本，並把目前的目標（target）傳過去
                Tower_Proj projectile = projectileGO.GetComponent<Tower_Proj>();
                if (projectile != null)
                {
                    projectile.Seek(target);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);

            if (target != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, target.position);
            }

        }
    }
}