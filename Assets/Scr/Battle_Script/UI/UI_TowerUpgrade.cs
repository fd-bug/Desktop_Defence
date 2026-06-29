using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class UI_TowerUpgrade : MonoBehaviour
{
    private Build_Node targetNode; // 🎯 記錄是哪一個地格打開了這個 UI

    [Header("UI 元件")]
    public Button upgradeButton;
    public TextMeshProUGUI costText; // 顯示需要多少錢（選填）

    void Start()
    {
        // 🎯 魔法程式碼：抓取按鈕或底圖的 Graphic 組件，直接動態複製材質並把 ZTest 改成 Always!
        Graphic graphic = GetComponent<Graphic>();
        if (graphic == null) graphic = GetComponentInChildren<Graphic>();

        if (graphic != null && graphic.material != null)
        {
            // 複製一份材質，避免改到全域的 UI/Default
            Material updatedMaterial = new Material(graphic.material);

            // 🔑 6 代表 UnityEngine.Rendering.CompareFunction.Always
            updatedMaterial.SetInt("_ZTest", 6);

            graphic.material = updatedMaterial;
        }
    }

    // 🎯 核心門戶：當地格生成這個 UI 時，必須呼叫這個方法來把「地格自己」傳進來綁定
    public void SetupUI(Build_Node node)
    {
        targetNode = node;

        // 順便更新按鈕上顯示的花費文字（如果以後需要的話）
        if (costText != null)
        {
            costText.text = $"${targetNode.GetUpgradeCost()}";
        }

        // 🎯 綁定按鈕點擊事件：當玩家按下 Upgrade 時，執行 UpgradeClicked
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(UpgradeClicked);
    }

    // 🖱️ 當玩家按下「Upgrade!」藍色對話框時觸發
    private void UpgradeClicked()
    {
        if (targetNode == null) return;

        // 呼叫地格執行真正的升級邏輯
        targetNode.AttemptUpgrade();

        // 升級完後，更新一下文字（或者直接關閉 UI）
        Build_Node.CloseActiveUI();
    }
}