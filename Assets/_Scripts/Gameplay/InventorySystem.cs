// InventorySystem.cs (���ع����㲥ͨ�������¼�)
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
        // �������޸ġ�����"��������Ѽ���"�¼�
        EventManager.Instance.Subscribe<PlayerData>("OnPlayerDataLoaded", LoadInventoryFromData);
    }
    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<PlayerData>("OnPlayerDataLoaded", LoadInventoryFromData);
        }
    }
    // �������޸ġ��˷����������¼��ص�
    public void LoadInventoryFromData(PlayerData playerData)
    {
        inventoryItems.Clear();
        Debug.Log($"[InventorySystem] ��ʼ����PlayerDataˢ�±������浵���� {playerData.inventoryItems.Count} ����Ʒ��");

        foreach (var itemSlot in playerData.inventoryItems)
        {
            // ��������־��
            Debug.Log($"[InventorySystem] ���ڳ��Լ��� GUID: {itemSlot.itemGuid}");
            ItemData itemData = AssetManager.Instance.GetAssetByGUID<ItemData>(itemSlot.itemGuid);
            if (itemData != null)
            {
                inventoryItems[itemData] = itemSlot.amount;
                // ��������־��
                Debug.Log($"[InventorySystem] -> �ɹ����ز������Ʒ: {itemData.itemName}, ����: {itemSlot.amount}");
            }
            else
            {
                // ��������־��
                Debug.LogWarning($"[InventorySystem] -> ����ʧ�ܣ��Ҳ���GUIDΪ {itemSlot.itemGuid} ����Ʒ�ʲ�������DataManager�е�GUID�Ƿ���ȷ���Լ���Ӧ���ʲ��Ƿ���ڡ�");
            }
        }
        Debug.Log($"[InventorySystem] �����Ѹ��������������ˢ�£���ǰ��Ʒ����: {inventoryItems.Count}");
        // ˢ������ٹ㲥һ��ͨ�ø����¼���֪ͨUI
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

        // �㲥UI�����¼�
        EventManager.Instance.TriggerEvent<object>(GameEvents.OnInventoryUpdated, null);
        // ���ؼ��޸ġ��㲥ͨ�õ���������¼�
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