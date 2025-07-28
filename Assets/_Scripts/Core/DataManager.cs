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
        PlayerData = !string.IsNullOrEmpty(json)
         ? JsonUtility.FromJson<PlayerData>(json)
         : new PlayerData();

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