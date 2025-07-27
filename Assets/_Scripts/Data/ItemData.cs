// ItemData.cs
using UnityEngine;

/// <summary>
/// 物品数据模板，使用ScriptableObject实现。
/// 这允许美术或策划在Unity编辑器中直接创建和配置物品，无需编写代码。
/// 实现了【数据与逻辑分离】的核心设计哲学。
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "梦境小镇/数据/物品数据")] // 根据总监要求，提供清晰的菜单路径
public class ItemData : ScriptableObject
{
    [Header("核心信息")]
    public string itemName; // 物品名称
    public Sprite itemIcon; // 物品图标 (2D游戏使用Sprite)

    [Header("描述")]
    [TextArea(3, 5)] // 提供一个更方便编辑的多行文本框
    public string description; // 物品描述

    // 可以在此扩展更多属性，如：
    // public ItemType itemType;
    // public int maxStackSize = 99;
    // public int price;
}
