using UnityEngine;

public class UI_Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // 抓取主相機的位置
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // 🎯 核心魔法：強迫 UI 的前方向（Forward）永遠與相機的前方向完全平行
            // 這會完美抵消透視投影造成的左右「平移/飄走」現象
            transform.rotation = mainCameraTransform.rotation;
        }
    }
}