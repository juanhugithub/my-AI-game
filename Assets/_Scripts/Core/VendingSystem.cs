// VendingSystem.cs
using System.Collections.Generic;
using UnityEngine;

public class VendingSystem : MonoBehaviour
{
    public static VendingSystem Instance { get; private set; }
    private void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    // 这是由交互脚本调用的简化版出售方法
    public void SellAllSellableItemsFromInventory(ShippingBinController bin)
    {
        long totalIncome = 0;
        var playerItems = InventorySystem.Instance.Items;
        List<ItemData> itemsToRemove = new List<ItemData>();

        foreach (var itemPair in playerItems)
        {
            ItemData item = itemPair.Key;
            if (item.sellPrice > 0)
            {
                totalIncome += (long)item.sellPrice * itemPair.Value;
                itemsToRemove.Add(item);
            }
        }

        if (totalIncome > 0)
        {
            // 【新增】广播出售事件，用于任务追踪
            foreach (var item in itemsToRemove)
            {
                EventManager.Instance.TriggerEvent(GameEvents.OnQuestObjectiveProgress, (ObjectiveType.SELL_ITEM, item.guid));
            }
            foreach (var item in itemsToRemove)
            {
                InventorySystem.Instance.RemoveItem(item, InventorySystem.Instance.Items[item]);
            }
            DataManager.Instance.AddGold(totalIncome);
            UImanager.Instance.ShowGlobalHint($"售出物品，获得 {totalIncome} 金币！");
        }
        else
        {
            UImanager.Instance.ShowGlobalHint("背包里没有可出售的物品。");
        }
    }
}