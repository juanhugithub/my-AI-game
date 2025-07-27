// EconomySystem.cs
using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    private void Start()
    {
        // 确保 EventManager 存在再订阅
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Subscribe<int>(GameEvents.OnMiniGameFinished, HandleMiniGameFinished);
        }
    }

    private void OnDestroy()
    {
        // 确保 EventManager 存在再取消订阅
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<int>(GameEvents.OnMiniGameFinished, HandleMiniGameFinished);
        }
    }

    /// <summary>
    /// 处理小游戏结束事件的回调函数。
    /// </summary>
    /// <param name="score">从事件中获得的分数</param>
    private void HandleMiniGameFinished(int score)
    {
        // 1. 调用DataManager增加金币。现在这个方法已经被恢复了。
        DataManager.Instance.AddGold(score);

        // 2. 逻辑处理完成，广播一个新的“金币已更新”事件，并附上新的金币总数
        long newGoldTotal = DataManager.Instance.PlayerData.Gold;
        EventManager.Instance.TriggerEvent(GameEvents.OnGoldUpdated, newGoldTotal);
    }
}
