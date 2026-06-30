using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 用於更新畫面左下角的波次文字

public class Wave_Manager : MonoBehaviour
{
    // 🎯 1. 定義單一隻/一小群怪物的生成指令
    [System.Serializable]
    public struct SpawnInstruction
    {
        public string label;             // 備忘標籤 (例如: "3隻重裝盆栽怪")
        public GameObject enemyPrefab;   // 要生成的怪物 Prefab (掛載 Enemy_Stats 的不同 Prefab)
        public int count;                // 生成數量
        public float spawnInterval;      // 這群怪物的出生間隔時間 (秒)
    }

    // 🎯 2. 定義一個「完整波次」的資料結構
    [System.Serializable]
    public struct WaveData
    {
        public string waveName;                   // 波次名稱 (例如 "Wave 2 - 混合突襲")
        public List<SpawnInstruction> spawns;     // 這一波裡面包含哪些怪物的生成指令
    }

    [Header("波次總清單")]
    public List<WaveData> waves;         // 🎯 在 Inspector 面板自由配置所有波次

    [Header("生成位置設定")]
    public Transform spawnPoint;         // 怪物出生點

    [Header("UI 顯示對接")]
    public TextMeshProUGUI waveText;     // 畫面上顯示 "WAVE 1" 的文字

    private int currentWaveIndex = 0;    // 目前是第幾波
    private bool isSpawning = false;     // 是否正在波次生成中

    void Start()
    {
        UpdateWaveUI();
    }

    void Update()
    {
        // 🎯 備用快捷鍵測試：按下鍵盤 "G" 鍵（或點擊你的 Next Wave 按鈕）啟動
        if (Input.GetKeyDown(KeyCode.G) && !isSpawning)
        {
            StartNextWave();
        }
    }

    // 🎯 呼叫啟動下一波 (綁定右下角 Next Wave 按鈕)
    public void StartNextWave()
    {
        if (isSpawning) return; // 正在生怪時不能重疊呼叫

        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWaveRoutine(waves[currentWaveIndex]));
        }
        else
        {
            Debug.Log("🎉 所有設計的波次已結束！恭喜通關！");
        }
    }

    // ⏳ 協程：精準處理一波之內「所有指令」的依序生成
    private IEnumerator SpawnWaveRoutine(WaveData wave)
    {
        isSpawning = true;
        Debug.Log($"🚀 【{wave.waveName}】正式開始！");

        UpdateWaveUI();

        // 依序執行這一波裡面的每一條生成指令
        foreach (SpawnInstruction instruction in wave.spawns)
        {
            if (instruction.enemyPrefab == null) continue;

            Debug.Log($"👾 正在生成指令：{instruction.label}，共 {instruction.count} 隻");

            for (int i = 0; i < instruction.count; i++)
            {
                if (spawnPoint != null)
                {
                    // 🎯 核心生成：直接把掛著同一個 Enemy_Stats 的特定 Prefab 實例化出來
                    Instantiate(instruction.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                }

                // 每生一隻，就等待設定的間隔秒數
                yield return new WaitForSeconds(instruction.spawnInterval);
            }
        }

        // 該波次的所有怪物「生完了」（注意：不代表場上怪被殺光，只是生完命令）
        currentWaveIndex++;
        isSpawning = false;

        Debug.Log($"🧹 第 {currentWaveIndex} 波怪物已全數出生完畢。");
    }

    // 📝 更新 Wave 文字
    private void UpdateWaveUI()
    {
        if (waveText != null)
        {
            // 畫面顯示目前的波次進度
            waveText.text = $"WAVE: {currentWaveIndex + 1}";
        }
    }
}