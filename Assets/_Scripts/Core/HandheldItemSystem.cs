// HandheldItemSystem.cs
using UnityEngine;

/// <summary>
/// 核心系统：手持物品管理器。
/// 作为一个全局单例，它负责追踪玩家当前从快捷栏中选中的物品是什么。
/// </summary>
public class HandheldItemSystem : MonoBehaviour
{
    /// <summary>
    /// 全局唯一的静态实例，方便任何脚本访问。
    /// </summary>
    public static HandheldItemSystem Instance { get; private set; }

    /// <summary>
    /// 公开属性，供其他脚本（如FarmPlotController）查询当前手持的物品。
    /// 它的set是私有的，只能由本系统内部修改。
    /// </summary>
    public ItemData HeldItem { get; private set; }

    // Unity的生命周期方法，在游戏对象被唤醒时调用
    private void Awake()
    {
        // 标准的单例模式实现，确保全局只有一个实例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 如果场景中已存在一个实例，则销毁后来者
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 设置当前手持的物品。
    /// 这个方法是提供给UI系统（如ActionBarUI）在玩家点击槽位时调用的。
    /// </summary>
    /// <param name="item">玩家选中的物品。如果传入null，则代表玩家变为空手。</param>
    public void SetHeldItem(ItemData item)
    {
        HeldItem = item;

        // 为了方便调试，我们在控制台打印出当前手持的物品
        // 使用三元运算符来处理item可能为null的情况
        Debug.Log($"[Handheld] 当前手持: {(item != null ? item.itemName : "空手")}");
    }
}