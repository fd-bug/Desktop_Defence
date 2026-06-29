using UnityEngine;
using UnityEngine.EventSystems;

// 🎯 同樣繼承點擊介面，只要點到這個物件，就全域關閉升級 UI
public class ClickToCloseUI : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging) return;

        // 🎯 直接呼叫剛剛在 BuildNode 寫好的靜態關閉方法
        Build_Node.CloseActiveUI();
    }
}