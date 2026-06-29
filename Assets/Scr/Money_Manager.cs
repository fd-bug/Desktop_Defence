using UnityEngine;
using TMPro; // 🎯 記得引入 TextMeshPro 命名空間

public class Money_Manager : MonoBehaviour
{
    // ✨ 單例模式，方便怪物死掉時直接呼叫
    public static Money_Manager instance;

    [Header("經濟設定")]
    public int startGold = 0;     // 遊戲開始時的初始金幣
    public int currentGold;        // 玩家當前擁有的金幣

    [Header("🎯 串接 UI 文字 (TMP)")]
    public TextMeshProUGUI goldText; // 👉 把剛剛建立的 Text_GoldCount 拉到這裡！

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 遊戲開始，初始化金幣
        currentGold = startGold;
        UpdateGoldUI();
    }

    // 💰 增加金幣的公開方法（擊殺怪物時呼叫）
    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"🪙 金幣增加！當前金幣：{currentGold}");

        // 🔄 即時更新 UI
        UpdateGoldUI();
    }

    // 💸 扣除金幣的公開方法（未來蓋防禦塔扣錢時可以用！）
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            return true; // 扣錢成功
        }
        else
        {
            Debug.LogWarning("❌ 金幣不足，無法購買/蓋塔！");
            return false; // 金幣不足
        }
    }

    // 🔄 更新 UI 文字顯示
    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = currentGold.ToString();
            // 如果你想帶字樣，也可以寫成：goldText.text = "Gold: " + currentGold;
        }
    }
}