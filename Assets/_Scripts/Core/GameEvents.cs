// GameEvents.cs

/// <summary>
/// 静态类，用于统一管理游戏中所有的事件名称字符串。
/// 使用此类的常量代替直接写入字符串（"魔术字符串"），
/// 可以有效避免因拼写错误导致的事件无法触发问题，并提高代码的可维护性。
/// 这是架构中【可扩展性与未来隐患】审查点的核心实践。
/// </summary>
public static class GameEvents
{
    // 当小游戏完成并产生分数时触发
    // 参数: int (分数)
    public const string OnMiniGameFinished = "OnMiniGameFinished";
    // 当玩家完成任务时触发
    public const string OnQuestStateChanged = "OnQuestStateChanged";
    // 当玩家金币数量更新后触发
    // 参数: long (新的金币总数)
    public const string OnGoldUpdated = "OnGoldUpdated";

    // 当背包内容发生变化时触发
    // 无参数，仅作为通知，UI收到后会主动向InventorySystem查询最新数据
    public const string OnInventoryUpdated = "OnInventoryUpdated";

    /// <summary>
    /// 当一个场景被叠加加载或卸载时触发。
    /// 参数: string (场景名称), SceneStateType (加载/卸载状态)
    /// </summary>
    public const string OnSceneStateChanged = "OnSceneStateChanged";
    /// <summary>
    /// 场景状态类型枚举，用于指示场景加载或卸载的类型。
    /// </summary>
    public enum SceneStateType { LoadedAdditive, UnloadedAdditive }
    /// <summary>
    /// 【新增】当任何可能影响任务进度的行为发生时触发。
    /// 这是新的、通用的任务进度事件。
    /// 参数: (ObjectiveType type, string targetID)
    /// </summary>
    public const string OnQuestObjectiveProgress = "OnQuestObjectiveProgress";
}


