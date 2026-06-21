using System.Collections.Generic; // 記得引入，才能使用 List
using UnityEngine;

public class Build_Manager : MonoBehaviour
{
    public static Build_Manager instance;
    public Transform buildAreaTransform;
    [Header("防禦塔預製物")]
    public GameObject towerPrefab; // 你的筆（Tower）Prefab

    [Header("格子尺寸設定")]
    public float gridSize = 0.21f;   // 維持你實測最完美的 0.21f
    public LayerMask gridLayer;     // 格子的 LayerMask

    private Camera mainCamera;

    // 用來記錄所有已建置防禦塔的「修正後中心點位置」
    private List<Vector3> occupiedPositions = new List<Vector3>();

    void Awake()
    {
        instance = this;
        mainCamera = Camera.main; //
    }

    public void TryBuildTower(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition); //
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, gridLayer)) //
        {
            // 1. 取得撞擊點的 3D 座標，並透過修正公式精確推導到「方框正中心」
            Vector3 spawnPosition = SnapToCorrectCenter(hit.point);

            // 2. 檢查這個中心點是不是已經有塔了
            if (IsPositionOccupied(spawnPosition))
            {
                Debug.LogWarning($"這個格子中心 {spawnPosition} 已經有防禦塔了，不可重複建造！");
                return; // 攔截，不給蓋
            }

            // 3. 在完美的方框中心生成防禦塔
            if (towerPrefab != null)
            {
                Instantiate(towerPrefab, spawnPosition, Quaternion.identity);
                Debug.Log($"成功在格子中心 {spawnPosition} 建造防禦塔！");

                // 4. 建造成功，記錄這個中心點
                occupiedPositions.Add(spawnPosition);
            }
        }
        else
        {
            Debug.Log("未拖曳到有效的建造格子區域內"); //
        }
    }

    bool IsPositionOccupied(Vector3 targetPos)
    {
        foreach (Vector3 pos in occupiedPositions)
        {
            // 用 0.05f 的微小距離來判斷是否在同一個格子中心
            if (Vector3.Distance(pos, targetPos) < 0.05f)
            {
                return true;
            }
        }
        return false;
    }


    Vector3 SnapToCorrectCenter(Vector3 hitPoint)
    {
        // 先四捨五入到最近的線條交接點（你實測最穩定的基底）
        int indexX = Mathf.RoundToInt(hitPoint.x / gridSize);
        int indexZ = Mathf.RoundToInt(hitPoint.z / gridSize);

        // 算出線條交接點的世界座標
        float snapX = indexX * gridSize;
        float snapZ = indexZ * gridSize;

        // ====== 【核心修正點】 ======
        // 依照你的實測，中心點需要向 -X 軸移動半個格子 (~0.1f)
        snapX -= (gridSize / 2f);
        // ===========================

        // 保持高度，稍微抬高一點避免穿模
        float snapY = hitPoint.y + 0.01f; //

        return new Vector3(snapX, snapY, snapZ);
    }
}