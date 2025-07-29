// ShopSystem.cs
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }
    private void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    public bool PurchaseItem(ItemData item, int amount = 1)
    {
        if (item == null) return false;

        long totalCost = (long)item.buyPrice * amount;

        // 1. 检查金币是否足够
        if (DataManager.Instance.PlayerData.Gold < totalCost)
        {
            UImanager.Instance.ShowGlobalHint("金币不足！");
            return false;
        }

        // 2. 交易
        DataManager.Instance.RemoveGold(totalCost);
        InventorySystem.Instance.AddItem(item, amount);

        // 3. 广播通用任务事件，用于追踪“购买物品”任务
        EventManager.Instance.TriggerEvent(GameEvents.OnQuestObjectiveProgress, (ObjectiveType.BUY_ITEM, item.guid));

        UImanager.Instance.ShowGlobalHint($"购买成功: {item.itemName} x{amount}");
        return true;
    }
}