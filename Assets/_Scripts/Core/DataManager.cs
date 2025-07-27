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
        if (Instance == null)
        {
            Instance = this;
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
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
            // ����д浵��ֱ�Ӽ���
            PlayerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            // ���ؼ�������: ���û�д浵���򴴽�һ����������ݣ������Ͳ�����Ʒ��
            // ���ǽ�����볡ȯ���㡱����ĺ��ġ�
            Debug.Log("[DataManager] δ�ҵ��浵�����ڴ�������Ҳ����Ͳ�����Ʒ...");
            PlayerData = new PlayerData();
            // �����������ﰴ����ӳ�ʼ��Ʒ
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemID = "VitalityFruit", amount = 5 });
           
        }
        // ������PlayerData�󣬱������е�storageBoxData���ҵ���Ӧ��StorageBoxController���ָ�������
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