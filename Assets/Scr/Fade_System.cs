using UnityEngine;
using System.Collections;

namespace fdbug
{
    /// <summary>
    /// 淡入淡出
    /// </summary>

    public class Fade_System
    /// <summary>
    /// 淡入淡出
    /// </summary>
    /// <param name="group">要淡入淡出的CanvasGroup</param>
    /// <param name="IsFadeIn">是否淡入，預設為true（淡入）</param>
    /// <param name="interval">每次改變alpha值的間隔時間，預設為0.03秒</param>
    {
        public static IEnumerator Fade(CanvasGroup group, bool IsFadeIn = true, float interval = 0.03f)

        {
            float increase = IsFadeIn ? 0.1f : -0.1f; //根據是否淡入來決定alpha值的增減方向
            for (int i = 0; i < 10; i++)
            {
                group.alpha += increase;
                yield return new WaitForSeconds(interval);

            }
            group.interactable = IsFadeIn; //淡入時允許互動，淡出時禁止互動
            group.blocksRaycasts = IsFadeIn; //淡入時允許射線穿透，淡出時禁止射線穿透

        }
    }
}
