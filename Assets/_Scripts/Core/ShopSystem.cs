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

        // 1. ������Ƿ��㹻
        if (DataManager.Instance.PlayerData.Gold < totalCost)
        {
            UImanager.Instance.ShowGlobalHint("��Ҳ��㣡");
            return false;
        }

        // 2. ����
        DataManager.Instance.RemoveGold(totalCost);
        InventorySystem.Instance.AddItem(item, amount);

        // 3. �㲥ͨ�������¼�������׷�١�������Ʒ������
        EventManager.Instance.TriggerEvent(GameEvents.OnQuestObjectiveProgress, (ObjectiveType.BUY_ITEM, item.guid));

        UImanager.Instance.ShowGlobalHint($"����ɹ�: {item.itemName} x{amount}");
        return true;
    }
}