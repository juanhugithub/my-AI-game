// VendingSystem.cs
using System.Collections.Generic;
using UnityEngine;

public class VendingSystem : MonoBehaviour
{
    public static VendingSystem Instance { get; private set; }
    private void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    // �����ɽ����ű����õļ򻯰���۷���
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
            // ���������㲥�����¼�����������׷��
            foreach (var item in itemsToRemove)
            {
                EventManager.Instance.TriggerEvent(GameEvents.OnQuestObjectiveProgress, (ObjectiveType.SELL_ITEM, item.guid));
            }
            foreach (var item in itemsToRemove)
            {
                InventorySystem.Instance.RemoveItem(item, InventorySystem.Instance.Items[item]);
            }
            DataManager.Instance.AddGold(totalIncome);
            UImanager.Instance.ShowGlobalHint($"�۳���Ʒ����� {totalIncome} ��ң�");
        }
        else
        {
            UImanager.Instance.ShowGlobalHint("������û�пɳ��۵���Ʒ��");
        }
    }
}