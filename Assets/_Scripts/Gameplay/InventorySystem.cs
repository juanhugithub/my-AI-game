// InventorySystem.cs (已重构，广播通用任务事件)
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private readonly Dictionary<ItemData, int> inventoryItems = new Dictionary<ItemData, int>();
    public IReadOnlyDictionary<ItemData, int> Items => inventoryItems;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    private void Start()
    {
        // 【核心修改】订阅"玩家数据已加载"事件
        EventManager.Instance.Subscribe<PlayerData>("OnPlayerDataLoaded", LoadInventoryFromData);
    }
    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<PlayerData>("OnPlayerDataLoaded", LoadInventoryFromData);
        }
    }
    // 【核心修改】此方法现在是事件回调
    public void LoadInventoryFromData(PlayerData playerData)
    {
        inventoryItems.Clear();
        Debug.Log($"[InventorySystem] 开始根据PlayerData刷新背包，存档中有 {playerData.inventoryItems.Count} 种物品。");

        foreach (var itemSlot in playerData.inventoryItems)
        {
            // 【新增日志】
            Debug.Log($"[InventorySystem] 正在尝试加载 GUID: {itemSlot.itemGuid}");
            ItemData itemData = AssetManager.Instance.GetAssetByGUID<ItemData>(itemSlot.itemGuid);
            if (itemData != null)
            {
                inventoryItems[itemData] = itemSlot.amount;
                // 【新增日志】
                Debug.Log($"[InventorySystem] -> 成功加载并添加物品: {itemData.itemName}, 数量: {itemSlot.amount}");
            }
            else
            {
                // 【新增日志】
                Debug.LogWarning($"[InventorySystem] -> 加载失败！找不到GUID为 {itemSlot.itemGuid} 的物品资产！请检查DataManager中的GUID是否正确，以及对应的资产是否存在。");
            }
        }
        Debug.Log($"[InventorySystem] 背包已根据最新玩家数据刷新，当前物品种类: {inventoryItems.Count}");
        // 刷新完后，再广播一个通用更新事件，通知UI
        EventManager.Instance.TriggerEvent<object>(GameEvents.OnInventoryUpdated, null);
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (item == null) return;

        if (inventoryItems.ContainsKey(item))
        {
            inventoryItems[item] += amount;
        }
        else
        {
            inventoryItems[item] = amount;
        }

        // 广播UI更新事件
        EventManager.Instance.TriggerEvent<object>(GameEvents.OnInventoryUpdated, null);
        // 【关键修改】广播通用的任务进度事件
        EventManager.Instance.TriggerEvent(GameEvents.OnQuestObjectiveProgress, (ObjectiveType.GATHER, item.guid));
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        if (item != null && inventoryItems.TryGetValue(item, out int currentAmount))
        {
            return currentAmount >= amount;
        }
        return false;
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        if (HasItem(item, amount))
        {
            inventoryItems[item] -= amount;
            if (inventoryItems[item] <= 0)
            {
                inventoryItems.Remove(item);
            }
            EventManager.Instance.TriggerEvent<object>(GameEvents.OnInventoryUpdated, null);
        }
    }

    public List<InventoryItemSlot> GetInventoryForSave()
    {
        var saveList = new List<InventoryItemSlot>();
        foreach (var itemPair in inventoryItems)
        {
            saveList.Add(new InventoryItemSlot
            {
                itemGuid = itemPair.Key.guid,
                amount = itemPair.Value
            });
        }
        return saveList;
    }
}