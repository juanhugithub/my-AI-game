// DataManager.cs
using System.Collections.Generic;
using System.Linq; // 引入Linq，方便后续操作
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
    // 【新增】当此对象被销毁时，检查它是否是当前的单例实例。
    // 如果是，则将静态实例设为null，防止其他脚本访问到已销毁的对象。
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
    // 【新增】从玩家数据中移除指定数量的金币。
    // 这是制造系统消耗金币所必需的核心功能。
    /// </summary>
    /// <param name="amount">要移除的金币数量</param>
    /// <returns>如果金币足够并成功移除则返回true，否则返回false。</returns>
    public bool RemoveGold(long amount)
    {
        if (amount < 0) return false; // 不能移除负数
        if (PlayerData.Gold >= amount)
        {
            PlayerData.Gold -= amount;
            SaveGame();
            Debug.Log($"[DataManager] 成功移除了 {amount} 金币, 剩余: {PlayerData.Gold}");
            return true;
        }
        else
        {
            Debug.LogWarning($"[DataManager] 尝试移除 {amount} 金币失败, 当前金币不足: {PlayerData.Gold}");
            return false;
        }
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
            // 您可以在这里按需添加初始物品
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemID = "VitalityFruit", amount = 5 });
           
        }
        // 加载完PlayerData后，遍历其中的storageBoxData，找到对应的StorageBoxController并恢复其数据
    }

    public void SaveGame()
    {
        // 【新增】保存所有储物箱数据
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