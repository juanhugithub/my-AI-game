// TownUI.cs
using UnityEngine;

/// <summary>
/// TownScene场景的UI交互脚本。
/// 【新增职责】：管理自身Canvas在叠加场景加载时的显示/隐藏。
/// </summary>
public class TownUI : MonoBehaviour
{
    [Header("大西瓜入口设置")]
    [SerializeField] private ItemData watermelonTicket; // 在Inspector中指定入场券

    // 【新增】引用自身Canvas，用于控制显示/隐藏
    [Header("UI控制")]
    [SerializeField] private Canvas townCanvas;

    private void Awake()
    {
        // 确保Canvas引用已设置
        if (townCanvas == null)
        {
            townCanvas = GetComponent<Canvas>();
            if (townCanvas == null)
            {
                Debug.LogError("[TownUI] 未找到Canvas组件或未在Inspector中指定！");
                enabled = false; // 禁用脚本防止后续错误
                return;
            }
        }
    }

    private void Start()
    {
        // 【新增】订阅场景状态变化事件
        if (EventManager.Instance != null)
        {
            // 使用Tuple作为事件参数
            EventManager.Instance.Subscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, HandleSceneStateChanged);
        }
    }

    private void OnDestroy()
    {
        // 【新增】取消订阅
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, HandleSceneStateChanged);
        }
    }

    /// <summary>
    /// 处理场景状态变化事件的回调。
    /// </summary>
    /// <param name="sceneInfo">包含场景名称和状态的元组</param>
    private void HandleSceneStateChanged((string sceneName, GameEvents.SceneStateType state) sceneInfo)
    {
        string sceneName = sceneInfo.sceneName;
        GameEvents.SceneStateType state = sceneInfo.state;

        // 判断被加载/卸载的场景是否是小游戏场景
        bool isMinigameScene = (sceneName == "MiniGame_TestScene" || sceneName == "WatermelonScene");

        if (!isMinigameScene) return; // 只关心小游戏场景的变化

        if (state == GameEvents.SceneStateType.LoadedAdditive)
        {
            // 如果小游戏场景被叠加加载，隐藏城镇UI
            townCanvas.gameObject.SetActive(false);
        }
        else if (state == GameEvents.SceneStateType.UnloadedAdditive)
        {
            // 如果小游戏场景被卸载，重新显示城镇UI
            townCanvas.gameObject.SetActive(true);
        }
    }

    // ... (原有 GoToMiniGame 和 GoToWatermelonGame 方法保持不变)
    public void GoToMiniGame()
    {
        SceneLoader.Instance.LoadSceneAsync("MiniGame_TestScene", true);
    }

    public void GoToWatermelonGame()
    {
        if (watermelonTicket == null)
        {
            Debug.LogError("[TownUI] 未在Inspector中设置大西瓜的入场券！");
            return;
        }

        if (InventorySystem.Instance.HasItem(watermelonTicket, 1))
        {
            InventorySystem.Instance.RemoveItem(watermelonTicket, 1);
            SceneLoader.Instance.LoadSceneAsync("WatermelonScene", true);
        }
        else
        {
            Debug.Log($"入场券不足！需要一个 {watermelonTicket.itemName}。");
        }
    }
}
