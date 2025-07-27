// StorageBoxController.cs (���ܲ����)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StorageBoxController : MonoBehaviour
{
    public string boxID = "DefaultBox_01";
    public List<InventoryItemSlot> storedItems = new List<InventoryItemSlot>();

    public void OpenStorage() { UImanager.Instance.OpenStoragePanel(this); }

    // ��������������Ʒ�б��ֻ������
    public IReadOnlyList<InventoryItemSlot> GetStoredItems() => storedItems;

    public void AddItem(ItemData item, int amount)
    {
        var itemSlot = storedItems.FirstOrDefault(slot => slot.itemID == item.name);
        if (itemSlot != null)
        {
            itemSlot.amount += amount;
        }
        else
        {
            storedItems.Add(new InventoryItemSlot { itemID = item.name, amount = amount });
        }
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        var itemSlot = storedItems.FirstOrDefault(slot => slot.itemID == item.name);
        if (itemSlot != null && itemSlot.amount >= amount)
        {
            itemSlot.amount -= amount;
            if (itemSlot.amount <= 0)
            {
                storedItems.Remove(itemSlot);
            }
            return true;
        }
        return false;
    }
}