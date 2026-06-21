using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拖拽外觀設定")]
    public Sprite pencilDragSprite; // 拖拽時想要顯示的去背鉛筆圖片

    private GameObject dragPreview;
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (pencilDragSprite == null)
        {
            Debug.LogWarning($"[{gameObject.name}] 未指派拖拽用的鉛筆圖片！將使用原本的 UI 圖標。");
        }

        // 1. 徹底換個方式：不複製自己，而是建立一個乾淨的新 UI 物件當作影子
        dragPreview = new GameObject("DragPreview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dragPreview.transform.SetParent(canvas.transform, false);

        // 2. 移除原本影子身上的拖拽腳本（因為是新建的，這步其實不用，但保持邏輯清晰）

        // 3. 關鍵步驟：設定乾淨的 Image 元件
        Image previewImage = dragPreview.GetComponent<Image>();
        if (previewImage != null && pencilDragSprite != null)
        {
            // 清除可能存在的舊材質或 Shader，確保去背透明度能正確顯示
            previewImage.material = null;
            previewImage.color = Color.white; // 確保顏色是乾淨的白色

            previewImage.sprite = pencilDragSprite;
            // 啟用 preserveAspect，防止鉛筆圖片被強制拉伸變形
            previewImage.preserveAspect = true;

            // 如果發現生成出來的鉛筆影子太小或太大，請解開下方這行並調整數字：
            // dragPreview.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100); 
        }

        // 4. 設定透明度並關閉 blocksRaycasts
        CanvasGroup group = dragPreview.AddComponent<CanvasGroup>();
        group.alpha = 0.7f;
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragPreview != null)
        {
            // 讓鉛筆影子精準跟著滑鼠走
            dragPreview.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Build_Manager.instance.TryBuildTower(eventData.position);

        if (dragPreview != null)
        {
            Destroy(dragPreview);
        }
    }
}