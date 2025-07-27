// InventorySystem.cs
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    private Dictionary<ItemData, int> inventoryItems = new Dictionary<ItemData, int>();
    public IReadOnlyDictionary<ItemData, int> Items => inventoryItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 【新增Start方法】: 在所有Awake执行完毕后，安全地从DataManager获取数据
    private void Start()
    {
        LoadInventoryFromDataManager();
    }

    private void LoadInventoryFromDataManager()
    {
        inventoryItems.Clear();
        var savedItems = DataManager.Instance.PlayerData.inventoryItems;
        Debug.Log($"[InventorySystem] 开始加载存档中的背包数据，共有 {savedItems.Count} 个槽位。"); // 新增日志

        foreach (var itemSlot in savedItems)
        {
            Debug.Log($"[InventorySystem] 尝试加载物品 ID: {itemSlot.itemID}, 数量: {itemSlot.amount}"); // 新增日志
            ItemData itemData = Resources.Load<ItemData>($"Items/{itemSlot.itemID}");
            if (itemData != null)
            {
                inventoryItems[itemData] = itemSlot.amount;
                Debug.Log($"[InventorySystem] 成功加载物品: {itemData.itemName} ({itemData.name}), 数量: {inventoryItems[itemData]}"); // 新增日志
            }
            else
            {
                Debug.LogWarning($"加载背包失败：找不到ID为 {itemSlot.itemID} 的物品。");
            }
        }
        // 初始加载完成后通知UI更新一次
        EventManager.Instance.TriggerEvent<object>(GameEvents.OnInventoryUpdated, null);
        Debug.Log($"[InventorySystem] 背包数据加载完成。当前背包物品数量: {inventoryItems.Count}"); // 新增日志
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (inventoryItems.ContainsKey(item))
        {
            inventoryItems[item] += amount;
        }
        else
        {
            inventoryItems[item] = amount;
        }
        EventManager.Instance.TriggerEvent<object>(GameEvents.OnInventoryUpdated, null);
        // 每次添加物品后，立即触发一次自动存档
        DataManager.Instance.SaveGame();
    }
    /// <summary>
    /// 【新增】检查背包中是否有足够数量的特定物品。
    /// </summary>
    public bool HasItem(ItemData item, int amount = 1)
    {
        if (inventoryItems.TryGetValue(item, out int currentAmount))
        {
            return currentAmount >= amount;
        }
        return false;
    }

    /// <summary>
    /// 【新增】从背包中移除指定数量的物品。
    /// </summary>
    public void RemoveItem(ItemData item, int amount = 1)
    {
        if (HasItem(item, amount))
        {
            inventoryItems[item] -= amount;
            // 如果物品数量降为0或以下，则从背包中彻底移除该物品
            if (inventoryItems[item] <= 0)
            {
                inventoryItems.Remove(item);
            }

            // 触发事件，通知UI等系统更新
            EventManager.Instance.TriggerEvent<object>(GameEvents.OnInventoryUpdated, null);
            DataManager.Instance.SaveGame();
        }
    }
    public List<InventoryItemSlot> GetInventoryForSave()
    {
        List<InventoryItemSlot> saveList = new List<InventoryItemSlot>();
        foreach (var itemPair in inventoryItems)
        {
            saveList.Add(new InventoryItemSlot
            {
                itemID = itemPair.Key.name,
                amount = itemPair.Value
            });
        }
        return saveList;
    }
}
