using UnityEngine;

public class Build_Manager : MonoBehaviour
{
    // 🎯 請檢查或在你的蓋塔管理器（如 Build_Manager.cs）中加入：
    public static Build_Manager instance;
    public int currentTowerCount = 0; // 🎯 紀錄全場當前防禦塔總數
    public int maxTowerLimit = 8;     // 🎯 最大限制 8 座

    void Awake()
    {
        instance = this;
    }

    // 🎯 提供一個方法讓地格可以查詢是不是滿了
    public bool CanBuildTower()
    {
        return currentTowerCount < maxTowerLimit;
    }
    [Header("防禦塔 Prefab 清單")]
    public GameObject[] towerPrefabs; // 0: 原子筆塔, 1: 盆栽塔

    [Header("射線偵測設定")]
    public LayerMask nodeLayer;       // 設為剛剛配置的 BuildNode 圖層

    private int selectedTowerIndex = 0; // 當前 UI 所選取要蓋的塔編號 (0 或 1)


    void Update()
    {
    }

    // 🎯 供 UI 按鈕呼叫：切換目前想蓋哪一種防禦塔（修正你原本的錯誤功能）
    public void SelectTowerToBuild(int towerIndex)
    {
        selectedTowerIndex = towerIndex;
        Debug.Log($"🎯 切換準備建造的防禦塔編號為: {selectedTowerIndex}");
    }

    private void HandleMouseClick()
    {
        // 從主攝影機發射一條射線指向滑鼠點擊的位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 只偵測屬於 nodeLayer (地塊) 的物件
        if (Physics.Raycast(ray, out hit, 100f, nodeLayer))
        {
            // 抓取被打中的那個地塊身上的 BuildNode 腳本
            Build_Node clickedNode = hit.collider.GetComponent<Build_Node>();

            if (clickedNode != null)
            {
                // 🪙 這裡未來可以結合金幣系統，例如：
                // int cost = 50;
                // if (Money_Manager.instance.TrySpendGold(cost)) { ... }

                // 🏗️ 執行在該地塊上蓋塔的動作
                GameObject towerToBuild = towerPrefabs[selectedTowerIndex];
                clickedNode.BuildTowerOnNode(towerToBuild);
            }
        }
    }
}