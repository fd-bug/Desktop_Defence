using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// 🎯 同時繼承開始拖拽、拖拽中、結束拖拽三個介面
public class UI_DragSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("防禦塔設定")]
    public int towerIndex = 0; // 🎯 0: 原子筆塔, 1: 盆栽塔

    [Header("拖拽外觀設定")]
    public Sprite DragSprite;  // 拖拽時想要顯示的去背圖片（例如原子筆或盆栽的精細圖）

    [Header("🎯 射線偵測設定")]
    public LayerMask nodeLayer; // 🎯 記得在 Inspector 設為你的 BuildNode 專用地塊圖層

    private GameObject dragPreview; // 拖拽時跟著滑鼠走的 UI 影子
    private Canvas canvas;

    void Start()
    {
        // 自動尋找父級的 Canvas，用來當作影子的家
        canvas = GetComponentInParent<Canvas>();
    }

    // ==================== 1. 開始拖拽：生成影子 ====================
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) canvas = GetComponentInParent<Canvas>();

        if (DragSprite == null)
        {
            Debug.LogWarning($"[{gameObject.name}] 未指派拖拽用的圖片！將無法顯示影子殘影。");
            return;
        }

        // 建立一個乾淨的新 UI 物件當作拖曳影子
        dragPreview = new GameObject("DragPreview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dragPreview.transform.SetParent(canvas.transform, false); //

        // 設定 Image 元件
        Image previewImage = dragPreview.GetComponent<Image>();
        if (previewImage != null)
        {
            previewImage.material = null; //
            previewImage.color = Color.white; //
            previewImage.sprite = DragSprite; //
            previewImage.preserveAspect = true; // 👉 啟用它防止防禦塔圖片被強制拉伸變形

            // 💡 如果你發現拉出來的格子影子太大或太小，可以解開下方這行手動調整寬高：
            // dragPreview.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        }

        // 設定 CanvasGroup 來防止影子擋住放開時的滑鼠射線
        CanvasGroup group = dragPreview.AddComponent<CanvasGroup>(); //
        group.alpha = 0.7f; // 讓影子帶有一點透明度
        group.blocksRaycasts = false; // 👉 關鍵：關閉射線阻擋，否則放開滑鼠時射線會打到影子自己！
    }

    // ==================== 2. 拖拽中：讓影子跟著滑鼠 ====================
    public void OnDrag(PointerEventData eventData)
    {
        if (dragPreview != null)
        {
            // 讓影子精準跟著滑鼠游標走
            dragPreview.transform.position = eventData.position;
        }
    }

    // ==================== 3. 結束拖拽：物理射線判定蓋塔 ====================
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[{gameObject.name}] 放開滑鼠，開始進行 3D 物理射線偵測...");

        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(currentMousePos);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 3f);

        if (Physics.Raycast(ray, out hit, 100f, nodeLayer))
        {
            Debug.Log($"🎯 1. 射線成功打中目標物件：{hit.collider.gameObject.name}");

            Build_Node targetNode = hit.collider.GetComponent<Build_Node>();

            if (targetNode != null)
            {
                Debug.Log($"💎 2. 成功取得該物件身上的 BuildNode 腳本。目前佔用狀態：{targetNode.isOccupied}");

                if (Build_Manager.instance == null)
                {
                    Debug.LogError("❌ 警告：場景中找不到 Build_Manager 的 static instance 單例！");
                    return;
                }

                if (!targetNode.isOccupied)
                {
                    Debug.Log($"📦 3. Build_Manager 存在，且地塊未被佔用。目前準備建造的 index 為：{towerIndex}，而 Build_Manager 的陣列長度為：{Build_Manager.instance.towerPrefabs.Length}");

                    if (towerIndex < Build_Manager.instance.towerPrefabs.Length)
                    {
                        GameObject towerPrefabToBuild = Build_Manager.instance.towerPrefabs[towerIndex];

                        if (towerPrefabToBuild == null)
                        {
                            Debug.LogError($"❌ 錯誤：Build_Manager 裡面的 towerPrefabs[{towerIndex}] 是空的(Null)！你忘記把 Prefab 拉進去陣列了！");
                            return;
                        }

                        Debug.Log($"🏗️ 4. 條件全數通過！即將呼叫地塊生成 Prefab: {towerPrefabToBuild.name}");
                        targetNode.BuildTowerOnNode(towerPrefabToBuild);
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ 卡關：UI 的 towerIndex ({towerIndex}) 超出了 Build_Manager 陣列的範圍！");
                    }
                }
                else
                {
                    Debug.LogWarning($"❌ 卡關：[{targetNode.name}] 已經有防禦塔了！");
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ 卡關：打中了 [{hit.collider.gameObject.name}]，但它身上沒有掛載 BuildNode.cs 腳本！");
            }
        }
        else
        {
            Debug.Log("❌ 射線放開時，完全沒有打中任何符合 Node Layer 設定的物件。");
        }

        if (dragPreview != null)
        {
            Destroy(dragPreview);
        }
    }
}