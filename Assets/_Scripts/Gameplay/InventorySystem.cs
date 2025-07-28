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

    public void LoadInventoryFromData(PlayerData playerData)
    {
        inventoryItems.Clear();
        foreach (var itemSlot in playerData.inventoryItems)
        {
            ItemData itemData = AssetManager.Instance.GetAssetByGUID<ItemData>(itemSlot.itemGuid);
            if (itemData != null)
            {
                inventoryItems[itemData] = itemSlot.amount;
            }
        }
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