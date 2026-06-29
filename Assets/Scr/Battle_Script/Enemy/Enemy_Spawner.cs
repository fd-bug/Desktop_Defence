using System.Collections;
using UnityEngine;
using UnityEngine.UI; // 🎯 【關鍵】必須引入 UI 命名空間才能操作 Button 物件

public class Enemy_Spawner : MonoBehaviour
{
    [Header("UI 按鈕綁定")]
    public Button startWaveButton;    // 🎯 宣告一個 Button 欄位，只需把 UI 按鈕拖進來

    [Header("怪物設定")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    [Header("固定波次設定")]
    public int enemyCount = 5;
    public float spawnInterval = 1f;

    private bool isSpawning = false;

    void Start()
    {
        // 🎯 【核心邏輯】用 Script 動態綁定按鈕點擊事件
        if (startWaveButton != null)
        {
            // AddListener 就像是跟按鈕說：「只要你被點擊，就立刻去執行 StartNextWave 這個方法」
            startWaveButton.onClick.AddListener(StartNextWave);
            Debug.Log("成功透過 Script 綁定波次按鈕！");
        }
        else
        {
            Debug.LogError("未指派 startWaveButton，Script 無法進行動態綁定！");
        }
    }

    // 💡 良好的寫法：當這個物件被摧毀時，順手把監聽拔掉，釋放記憶體防漏
    void OnDestroy()
    {
        if (startWaveButton != null)
        {
            startWaveButton.onClick.RemoveListener(StartNextWave);
        }
    }

    // 這是原本的方法，完全不需要修改
    public void StartNextWave()
    {
        if (isSpawning)
        {
            Debug.LogWarning("當前波次正在生成中，請稍候！");
            return;
        }
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        isSpawning = true;
        Debug.Log($"【波次開始】即將生成 {enemyCount} 隻怪物！");

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("【波次結束】所有預定怪物生成完畢！");
        isSpawning = false;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("❌ 錯誤：沒有指派 enemyPrefab！請檢查 Inspector。");
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("❌ 錯誤：沒有指派 spawnPoint！請檢查 Inspector。");
            return;
        }

        // 確定都有才生成
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"👾 成功在 Hierarchy 生成怪物物件：{newEnemy.name}，位置：{spawnPoint.position}");
    }
}