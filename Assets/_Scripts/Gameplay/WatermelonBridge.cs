// WatermelonBridge.cs (V4 - 最终逻辑版)
using UnityEngine;

/// <summary>
/// “大西瓜”玩法与“梦境小镇”主系统的桥梁（适配器）。
/// 【V4职责更新】：根据总监最终指示，清晰地区分“设置菜单”和“TopLine游戏结束”的逻辑。
/// 【V4.1职责更新】：修正“重新开始”按钮的场景加载逻辑，使其与项目叠加式架构兼容。
/// </summary>
public class WatermelonBridge : MonoBehaviour
{
    public static WatermelonBridge Instance { get; private set; }

    [SerializeField] private GameManager watermelonGameManager;
    [SerializeField] private ScoreManager watermelonScoreManager;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        if (watermelonGameManager == null) watermelonGameManager = FindObjectOfType<GameManager>();
        if (watermelonScoreManager == null) watermelonScoreManager = FindObjectOfType<ScoreManager>();
    }

    // --- 公共接口 ---

    /// <summary>
    /// 【新增】由TopLine触发的最终结算接口。
    /// 分数 1:1 兑换成金币。
    /// </summary>
    public void ReportFinalResult(int finalScore)
    {
        Debug.Log($"[WatermelonBridge] 收到最终游戏结果，得分: {finalScore}。将以1:1兑换金币。");

        // 触发主系统事件，EconomySystem会自动处理
        EventManager.Instance.TriggerEvent(GameEvents.OnMiniGameFinished, finalScore);

        // 延迟2秒（让玩家看一下分数），然后返回小镇
        Invoke(nameof(UnloadScene), 2f);
    }

    /// <summary>
    /// 【修改】处理“设置”面板中的“返回小镇”按钮点击。
    /// 结算分数，分数减半后兑换成金币，然后立即退出。
    /// </summary>
    public void HandleReturnToTownFromSettings()
    {
        Time.timeScale = 1f; // 必须先恢复时间！
        Debug.Log("[WatermelonBridge] 从设置菜单请求返回小镇并结算...");

        int currentScore = Mathf.FloorToInt(watermelonScoreManager.TotalScore);
        int scoreToConvert = currentScore / 2;
        Debug.Log($"当前得分: {currentScore}, 折半后可兑换金币: {scoreToConvert}");

        EventManager.Instance.TriggerEvent(GameEvents.OnMiniGameFinished, scoreToConvert);

        UnloadScene();
    }
    
    /// <summary>
    /// 【重大修改】处理“设置”面板中的“重新开始”按钮点击。
    ///  现在只需调用SceneLoader中封装好的、安全的方法即可。
    /// </summary>
    public void HandleRestartFromSettings()
    {
        Time.timeScale = 1f; // 恢复时间
        // 调用 SceneLoader 的新接口，将复杂逻辑委托给它
        SceneLoader.Instance.ReloadSceneAsync("WatermelonScene");
    }
    private void UnloadScene()
    {
        SceneLoader.Instance.UnloadSceneAsync("WatermelonScene");
    }
}