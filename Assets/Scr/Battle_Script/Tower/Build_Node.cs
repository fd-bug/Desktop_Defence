using UnityEngine;
using UnityEngine.EventSystems;

public class Build_Node : MonoBehaviour, IPointerClickHandler
{
    public bool isOccupied = false;
    private GameObject instantiatedTower;

    [Header("UI 設定")]
    public GameObject upgradeUIPrefab;
    public float uiHeightOffset = 0.3f;

    [Header("升級系統數值")]
    public int upgradeCount = 0;        // 🎯 目前已升級次數（上限 3 次）
    public int baseUpgradeCost = 50;    // 🎯 第一次升級需要的基礎金額

    public static GameObject currentActiveUI;

    public void BuildTowerOnNode(GameObject towerPrefab)
    {
        instantiatedTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
        instantiatedTower.transform.SetParent(this.transform);
        isOccupied = true;
        upgradeCount = 0; // 新蓋好時，升級次數歸零
    }

    // 🖱️ 當地格被滑鼠點擊時
    public void OnPointerClick(PointerEventData eventData)
    {
        // 🎯 漏洞防禦：如果是拖拽過程中放開、或者點擊了 UI 導致的誤觸，直接攔截
        if (eventData.dragging) return;

        // 🎯 如果目前是「蓋塔模式/手上有拖著東西」，我們就不應該彈出升級 UI 
        // (這能有效防止一邊拖塔一邊點格子的蓋塔次數 Bug，請根據你 Build_Manager 的變數名稱微調)
        // if (Build_Manager.instance.IsHoldingTower()) return; 

        CloseActiveUI();

        if (isOccupied && instantiatedTower != null)
        {
            if (upgradeCount >= 3) return;

            Debug.Log($"🏗️ 點擊了【{gameObject.name}】，準備彈出升級 UI！");

            // 1. 原本的基礎位置（物件正上方）
            Vector3 uiPosition = instantiatedTower.transform.position + Vector3.up * uiHeightOffset;

            // 2. 🎯 【新增】計算從地格指向主相機的方向
            Vector3 dirToCamera = (Camera.main.transform.position - instantiatedTower.transform.position).normalized;

            // 3. 🎯 【修正】往上飄的同時，順著相機方向拉近 0.5 單位，直接插隊到葉子前方！
            uiPosition += dirToCamera * 0.5f;

            currentActiveUI = Instantiate(upgradeUIPrefab, uiPosition, Quaternion.identity);
            currentActiveUI.transform.rotation = Camera.main.transform.rotation;

            UI_TowerUpgrade uiScript = currentActiveUI.GetComponent<UI_TowerUpgrade>();
            if (uiScript != null) uiScript.SetupUI(this);
        }
        else
        {
            Debug.Log("點擊空地格，關閉升級視窗。");
        }
    }

    // 🎯 計算目前這一次升級需要花多少錢（多點一次花費變高）
    public int GetUpgradeCost()
    {
        // 範例公式：每次升級變貴 1.5 倍（50 -> 75 -> 112）
        // 你也可以改成簡單的：baseUpgradeCost + (upgradeCount * 50)
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(1.5f, upgradeCount));
    }

    // 🎯 執行真正的金錢扣除與數值提升（第 3、4 步）
    public void AttemptUpgrade()
    {
        int currentCost = GetUpgradeCost();

        // 1. 判斷金錢足夠與否 (對接你的 Money_Manager)
        if (Money_Manager.instance != null && Money_Manager.instance.currentGold >= currentCost)
        {
            // 2. 扣錢
            Money_Manager.instance.AddGold(-currentCost); // 傳入負值代表扣錢

            // 3. 增加升級次數
            upgradeCount++;
            Debug.Log($"🪙 升級成功！花費 {currentCost}，目前已升級 {upgradeCount} 次。");

            // 4. 🎯 提高防禦塔的數值
            ApplyStatsImprovement();
        }
        else
        {
            // 5. 如不符合條件，無事發生
            Debug.Log("❌ 金幣不足，無法升級！ (播放失敗音效)");
            // AudioSource.PlayClipAtPoint(failSound, transform.position); 
        }
    }

    // 🎯 實際把攻擊力或範圍變強的地方
    private void ApplyStatsImprovement()
    {
        if (instantiatedTower == null) return;

        // 1. 嘗試抓取原子筆塔組件
        Tower_Aim penTower = instantiatedTower.GetComponent<Tower_Aim>();
        if (penTower != null)
        {
            // 🖊️ 呼叫原子筆的升級方法
            penTower.UpgradeTower();
            return;
        }

        // 2. 如果不是原子筆，嘗試抓取盆栽塔組件
        Plant_Tower plantTower = instantiatedTower.GetComponent<Plant_Tower>();
        if (plantTower != null)
        {
            // 🌵 呼叫盆栽的升級方法
            plantTower.UpgradeTower();
            return;
        }

        Debug.LogWarning($"⚠️ 在 [{instantiatedTower.name}] 上找不到可升級的防禦塔組件！");
    }

    public static void CloseActiveUI()
    {
        if (currentActiveUI != null)
        {
            Destroy(currentActiveUI);
            currentActiveUI = null;
        }
    }
}