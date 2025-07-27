// DataManager.cs
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

    public void AddGold(long amount)
    {
        if (amount <= 0) return;
        PlayerData.Gold += amount;
        SaveGame();
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
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemID = "VitalityFruit", amount = 5 });
        }
    }

    public void SaveGame()
    {
        if (InventorySystem.Instance != null)
        {
            PlayerData.inventoryItems = InventorySystem.Instance.GetInventoryForSave();
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