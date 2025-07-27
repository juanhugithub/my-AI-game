// PlayerData.cs (文件头部新增)
using System;
using System.Collections.Generic; // 需要引入

/// <summary>
/// 可序列化的背包物品槽位。
/// 用于在PlayerData中存储背包物品信息，因为它不能直接序列化字典。
/// 包含物品的ID和数量。
/// </summary>
[Serializable]
public class InventoryItemSlot
{
    public string itemID; // 我们将使用物品的 ScriptableObject 文件名作为唯一ID
    public int amount;
}

[Serializable]
public class PlayerData
{
    public long Gold;
    // 新增：背包物品列表
    public List<InventoryItemSlot> inventoryItems = new List<InventoryItemSlot>();
}