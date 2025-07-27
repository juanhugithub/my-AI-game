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

    // ������Start������: ������Awakeִ����Ϻ󣬰�ȫ�ش�DataManager��ȡ����
    private void Start()
    {
        LoadInventoryFromDataManager();
    }

    private void LoadInventoryFromDataManager()
    {
        inventoryItems.Clear();
        var savedItems = DataManager.Instance.PlayerData.inventoryItems;
        Debug.Log($"[InventorySystem] ��ʼ���ش浵�еı������ݣ����� {savedItems.Count} ����λ��"); // ������־

        foreach (var itemSlot in savedItems)
        {
            Debug.Log($"[InventorySystem] ���Լ�����Ʒ ID: {itemSlot.itemID}, ����: {itemSlot.amount}"); // ������־
            ItemData itemData = Resources.Load<ItemData>($"Items/{itemSlot.itemID}");
            if (itemData != null)
            {
                inventoryItems[itemData] = itemSlot.amount;
                Debug.Log($"[InventorySystem] �ɹ�������Ʒ: {itemData.itemName} ({itemData.name}), ����: {inventoryItems[itemData]}"); // ������־
            }
            else
            {
                Debug.LogWarning($"���ر���ʧ�ܣ��Ҳ���IDΪ {itemSlot.itemID} ����Ʒ��");
            }
        }
        // ��ʼ������ɺ�֪ͨUI����һ��
        EventManager.Instance.TriggerEvent<object>(GameEvents.OnInventoryUpdated, null);
        Debug.Log($"[InventorySystem] �������ݼ�����ɡ���ǰ������Ʒ����: {inventoryItems.Count}"); // ������־
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
        // ÿ�������Ʒ����������һ���Զ��浵
        DataManager.Instance.SaveGame();
    }
    /// <summary>
    /// ����������鱳�����Ƿ����㹻�������ض���Ʒ��
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
    /// ���������ӱ������Ƴ�ָ����������Ʒ��
    /// </summary>
    public void RemoveItem(ItemData item, int amount = 1)
    {
        if (HasItem(item, amount))
        {
            inventoryItems[item] -= amount;
            // �����Ʒ������Ϊ0�����£���ӱ����г����Ƴ�����Ʒ
            if (inventoryItems[item] <= 0)
            {
                inventoryItems.Remove(item);
            }

            // �����¼���֪ͨUI��ϵͳ����
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
