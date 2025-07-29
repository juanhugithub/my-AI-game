// DataManager.cs
using System.Collections.Generic;
using System.Linq; // ����Linq�������������
using UnityEngine;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public PlayerData PlayerData { get; private set; }
    private const string SaveKey = "DreamTownSave";

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    private void Start()
    {
        // ��LoadGame��Awake����Start����Ϊ˫�ر���
        // Start������Awake֮��ִ�У��ܽ�һ��ȷ������Instance����׼������
        LoadGame();
    }
    // �����������˶�������ʱ��������Ƿ��ǵ�ǰ�ĵ���ʵ����
    // ����ǣ��򽫾�̬ʵ����Ϊnull����ֹ�����ű����ʵ������ٵĶ���
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    public void AddGold(long amount)
    {
        if (amount <= 0) return;
        PlayerData.Gold += amount;
        SaveGame();
    }
    /// <summary>
    // ��������������������Ƴ�ָ�������Ľ�ҡ�
    // ��������ϵͳ���Ľ��������ĺ��Ĺ��ܡ�
    /// </summary>
    /// <param name="amount">Ҫ�Ƴ��Ľ������</param>
    /// <returns>�������㹻���ɹ��Ƴ��򷵻�true�����򷵻�false��</returns>
    public bool RemoveGold(long amount)
    {
        if (amount < 0) return false; // �����Ƴ�����
        if (PlayerData.Gold >= amount)
        {
            PlayerData.Gold -= amount;
            SaveGame();
            Debug.Log($"[DataManager] �ɹ��Ƴ��� {amount} ���, ʣ��: {PlayerData.Gold}");
            return true;
        }
        else
        {
            Debug.LogWarning($"[DataManager] �����Ƴ� {amount} ���ʧ��, ��ǰ��Ҳ���: {PlayerData.Gold}");
            return false;
        }
    }


    public void LoadGame()
    {
        string json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            PlayerData = JsonUtility.FromJson<PlayerData>(json);
            // ��������־��
            Debug.Log($"[DataManager] �ɹ����ؾɴ浵���浵�а��� {PlayerData.inventoryItems.Count} ����Ʒ��");
        }
        else
        {
            Debug.Log("[DataManager] δ�ҵ��浵�����ڴ������������...");
            PlayerData = new PlayerData();
            // Ϊ������ṩ��ʼ��Դ (��ȷ��GUID��ȷ)
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemGuid = "45e42e94-69db-435a-8553-71df0e14d90f", amount = 5 }); // ����"VitalityFruit"��GUID�����
                                                                                                                                    // ���޸ġ��ṩ20�Ż��������ӣ�ʹ����GUID
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemGuid = "3df0a020-13b5-4846-a7fa-4421b336ac5f", amount = 20 });
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemGuid = "572f5054-4142-4b05-b248-55a92b614b95", amount = 1 });
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemGuid = "582f5054-4142-4b05-b248-55a92b614b95", amount = 1 });
            PlayerData.Gold = 1000; // ��ʼ���
                                    // ��������־��
            Debug.Log($"[DataManager] ��Ϊ�������� {PlayerData.inventoryItems.Count} �ֳ�ʼ��Ʒ��");
            SaveGame(); // �����������ݺ����̱���һ��
        }
        // �������޸ġ����ۼ��ؾɵ����Ǵ����µ������㲥һ���¼���֪ͨ���й������ݵ�ϵͳ����ˢ��
        EventManager.Instance.TriggerEvent("OnPlayerDataLoaded", PlayerData);
        // ���������ڼ�����������ݺ���������InventorySystem�����ر���
        // ��ʱAssetManager��Awake�Ѿ�ִ����ϣ��ǰ�ȫ��
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.LoadInventoryFromData(PlayerData);
        }
    }

    public void SaveGame()
    {
        // ���������������д���������
        PlayerData.storageBoxData.Clear();
        var allBoxes = FindObjectsOfType<StorageBoxController>();
        foreach (var box in allBoxes)
        {
            PlayerData.storageBoxData.Add(new StorageData
            {
                boxID = box.boxID,
                items = new List<InventoryItemSlot>(box.storedItems)
            });
        }

        string json = JsonUtility.ToJson(PlayerData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}