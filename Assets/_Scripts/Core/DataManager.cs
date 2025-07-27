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
            // 如果有存档，直接加载
            PlayerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            // 【关键修正】: 如果没有存档，则创建一个新玩家数据，并赠送测试物品。
            // 这是解决“入场券不足”问题的核心。
            Debug.Log("[DataManager] 未找到存档。正在创建新玩家并赠送测试物品...");
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