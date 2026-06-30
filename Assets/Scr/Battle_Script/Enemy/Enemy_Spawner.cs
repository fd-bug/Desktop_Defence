using System.Collections;
using System.Collections.Generic; // 🎯 引入 List 需要的命名空間
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 🎯 如果你的波次文字是 TextMeshPro，可以解開相關註解

public class Enemy_Spawner : MonoBehaviour
{
    [Header("UI 按鈕綁定")]
    public Button startWaveButton;

    [Header("UI 文字對接 (選填)")]
    public TextMeshProUGUI waveText; // 如果想在畫面上更新 WAVE: 1 文字可以拉進來

    [Header("生成位置設定")]
    public Transform spawnPoint;

    // 🎯 1. 定義「單一波次」要生什麼怪、生幾隻的表格結構
    [System.Serializable]
    public struct WaveData
    {
        public string waveLabel;         // 備忘標籤 (例如: "第一波-原子筆怪小兵" 或 "第二波-盆栽重裝怪")
        public GameObject enemyPrefab;   // 這一波要使用的怪物 Prefab (身上都掛同一個 Enemy_Stats)
        public int enemyCount;           // 這一波要生成幾隻
        public float spawnInterval;      // 這波怪物的出生間隔時間 (秒)
    }

    // 🎯 2. 用 List 做出可以無限新增的「波次總清單」
    [Header("波次關卡總配置")]
    public List<WaveData> waves;

    private int currentWaveIndex = 0; // 🎯 紀錄目前是第幾波
    private bool isSpawning = false;

    void Start()
    {
        // 1. 動態綁定按鈕監聽
        if (startWaveButton != null)
        {
            startWaveButton.onClick.AddListener(StartNextWave);
            Debug.Log("成功透過 Script 綁定波次按鈕！");
        }
        else
        {
            Debug.LogError("未指派 startWaveButton，Script 無法進行動態綁定！");
        }

        // 🎯 2. 【核心初始化】進遊戲強制將波次歸零
        currentWaveIndex = 0;
        isSpawning = false;

        // 🎯 3. 【初始化文字】確保在進遊戲第一幀，立刻刷出 Day 1
        UpdateWaveUI();
    }
    void OnDestroy()
    {
        if (startWaveButton != null)
        {
            startWaveButton.onClick.RemoveListener(StartNextWave);
        }
    }

    // 🖱️ 點擊 Next Wave 按鈕觸發
    public void StartNextWave()
    {
        if (isSpawning)
        {
            Debug.LogWarning("當前波次正在生成中，請稍候！");
            return;
        }

        // 🎯 檢查是不是還有下一波可以跑
        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWaveRoutine());
        }
        else
        {
            Debug.Log("🎉 所有設計的波次都已通關！幹得好！");
        }
    }

    // ⏳ 讀取目前波次資料的協程
    private IEnumerator SpawnWaveRoutine()
    {
        isSpawning = true;

        // 🎯 抓取當前波次的表格資料
        WaveData currentWave = waves[currentWaveIndex];
        Debug.Log($"【{currentWave.waveLabel}】開始！預計生成 {currentWave.enemyCount} 隻怪物！");

        UpdateWaveUI();

        // 🎯 依據該波次設定的數量進行迴圈生成
        for (int i = 0; i < currentWave.enemyCount; i++)
        {
            SpawnEnemy(currentWave.enemyPrefab);
            yield return new WaitForSeconds(currentWave.spawnInterval);
        }

        Debug.Log($"【{currentWave.waveLabel}】所有預定怪物生成完畢！");

        // 🎯 生完之後，波次序號 +1，並解除生成鎖定
        currentWaveIndex++;
        isSpawning = false;
    }

    // 🎯 傳入特定種類的 Prefab 進行生成
    private void SpawnEnemy(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("波次設定中遺失了怪物 Prefab！");
            return;
        }

        if (spawnPoint != null)
        {
            // 實例化該波次指定的怪物
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
    }
    private void UpdateWaveUI()
    {
        // 🎯 安全防線一：如果玩家根本還沒在 Inspector 填寫任何波次，顯示預設值防出錯
        if (waves == null || waves.Count == 0)
        {
            if (waveText != null) waveText.text = "Day 1";
            return;
        }

        // 🎯 安全防線二：檢查目前的波次序號是否還在清單範圍內
        if (currentWaveIndex < waves.Count)
        {
            WaveData currentWave = waves[currentWaveIndex];

            if (waveText != null)
            {
                // 精準反映當前波次在 Inspector 填寫的標籤 (例如 "Day 1")
                waveText.text = currentWave.waveLabel;
                Debug.Log($"📝 UI 文字已成功更新為：{currentWave.waveLabel}");
            }
        }
        else
        {
            // 所有波次都撐過去了
            if (waveText != null)
            {
                waveText.text = "SURVIVED!";
            }
        }
    }
}