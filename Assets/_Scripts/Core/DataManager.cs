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
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    private void Start()
    {
        // 将LoadGame从Awake移至Start，作为双重保险
        // Start在所有Awake之后执行，能进一步确保所有Instance都已准备就绪
        LoadGame();
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
            PlayerData = JsonUtility.FromJson<PlayerData>(json);
            // 【新增日志】
            Debug.Log($"[DataManager] 成功加载旧存档。存档中包含 {PlayerData.inventoryItems.Count} 种物品。");
        }
        else
        {
            Debug.Log("[DataManager] 未找到存档，正在创建新玩家数据...");
            PlayerData = new PlayerData();
            // 为新玩家提供初始资源 (请确保GUID正确)
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemGuid = "45e42e94-69db-435a-8553-71df0e14d90f", amount = 5 }); // 假设"VitalityFruit"的GUID是这个
                                                                                                                                    // 【修改】提供20颗活力果种子，使用其GUID
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemGuid = "3df0a020-13b5-4846-a7fa-4421b336ac5f", amount = 20 });
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemGuid = "572f5054-4142-4b05-b248-55a92b614b95", amount = 1 });
            PlayerData.inventoryItems.Add(new InventoryItemSlot { itemGuid = "582f5054-4142-4b05-b248-55a92b614b95", amount = 1 });
            PlayerData.Gold = 1000; // 初始金币
                                    // 【新增日志】
            Debug.Log($"[DataManager] 已为新玩家添加 {PlayerData.inventoryItems.Count} 种初始物品。");
            SaveGame(); // 创建完新数据后立刻保存一次
        }
        // 【核心修改】无论加载旧档还是创建新档，都广播一个事件，通知所有关心数据的系统进行刷新
        EventManager.Instance.TriggerEvent("OnPlayerDataLoaded", PlayerData);
        // 【新增】在加载完玩家数据后，立即调用InventorySystem来加载背包
        // 此时AssetManager的Awake已经执行完毕，是安全的
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.LoadInventoryFromData(PlayerData);
        }
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