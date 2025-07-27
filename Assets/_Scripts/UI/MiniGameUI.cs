// MiniGameUI.cs
using UnityEngine;

/// <summary>
/// MiniGame_TestScene场景的UI交互脚本。
/// </summary>
public class MiniGameUI : MonoBehaviour
{
    // 在Unity编辑器中拖拽赋值
    [Header("奖励设置")]
    [SerializeField] private ItemData rewardItem;

    public void OnScoreButtonClick()
    {
        // 检查是否设置了奖励物品
        if (rewardItem != null)
        {
            // 直接调用背包系统添加物品，而不是广播分数事件
            InventorySystem.Instance.AddItem(rewardItem, 1);
        }
        else
        {
            Debug.LogError("未在MiniGameUI上设置奖励物品！");
        }
    }

    public void OnReturnButtonClick()
    {
        // 调用SceneLoader卸载自身场景
        SceneLoader.Instance.UnloadSceneAsync("MiniGame_TestScene");
    }
}
