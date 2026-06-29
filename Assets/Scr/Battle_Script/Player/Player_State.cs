using UnityEngine;
using UnityEngine.UI;

public class Player_Stats : MonoBehaviour
{
    // 單例模式，讓 EnemyPathing 可以直接呼叫
    public static Player_Stats instance;

    [Header("玩家生命值設定")]
    public int maxLives = 20;
    private int currentLives;

    [Header("串接血條 UI (Filler)")]
    public Image healthBarFiller; // 👉 把你的 Filler 圖片拉進這裡！

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        currentLives = maxLives;
        if (healthBarFiller != null) healthBarFiller.fillAmount = 1f;
    }

    // 💔 供怪物到達終點時呼叫
    public void TakeDamage(int damage)
    {
        currentLives -= damage;
        Debug.Log($"💔 玩家漏怪受傷！剩餘生命：{currentLives}/{maxLives}");

        // 🔄 更新 UI 血條的 Fill Amount
        if (healthBarFiller != null)
        {
            float healthPercent = (float)currentLives / (float)maxLives;
            healthBarFiller.fillAmount = Mathf.Clamp01(healthPercent);
        }

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("💀 GAME OVER! 遊戲結束。");
    }
}