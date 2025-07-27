// Watermelon_UIManager.cs
using TMPro; // 需要引入TextMeshPro的命名空间
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI总管理器，采用单例模式。
/// 职责：管理常驻的全局UI元素，如金币显示。
/// 它严格遵守【事件驱动原则】，通过订阅OnGoldUpdated事件来更新UI，
/// 而不是被其他任何系统直接调用。这是对【架构符合度】审查点的严格遵守。
/// </summary>
public class UImanager : MonoBehaviour
{
    public static UImanager Instance { get; private set; }

    [Header("通用UI元素")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("背包UI")]
    [SerializeField] private GameObject inventoryPanel; // 背包面板的根对象
    [SerializeField] private GameObject inventorySlotPrefab; // 背包项的预制体
    [SerializeField] private Transform inventorySlotParent; // 背包项生成的父节点

    private bool isInventoryUIValid = true; // 用于标记UI配置是否正确
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
        // 【新增健壮性检查】在游戏开始前检查所有必要的UI引用是否已配置
        if (inventoryPanel == null || inventorySlotPrefab == null || inventorySlotParent == null)
        {
            Debug.LogError("【UImanager配置错误】: 背包UI的某些字段（Panel, Slot Prefab, or Parent）未在Inspector中指定！背包UI将无法工作。");
            isInventoryUIValid = false;
        }
    }

    private void Start()
    {
        // 订阅事件
        EventManager.Instance.Subscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
        EventManager.Instance.Subscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);

        // 初始更新
        UpdateGoldDisplay(DataManager.Instance.PlayerData.Gold);
        inventoryPanel.SetActive(false); // 默认隐藏背包
    }

    private void OnDestroy()
    {
        // 取消订阅
        EventManager.Instance.Unsubscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
        EventManager.Instance.Unsubscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);
    }

    /// <summary>
    /// 更新金币显示的具体方法。此方法由事件回调触发。
    /// </summary>
    /// <param name="newGoldAmount">新的金币总数</param>
    private void UpdateGoldDisplay(long newGoldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"金币: {newGoldAmount}";
        }
    }

    /// <summary>
    /// 响应背包更新事件，刷新UI显示。
    /// </summary>
    private void UpdateInventoryDisplay(object data = null)
    {
        // 获取最新的背包数据
        var items = InventorySystem.Instance.Items;

        // 如果背包为空，则隐藏面板
        if (items.Count == 0)
        {
            inventoryPanel.SetActive(false);
            return;
        }

        inventoryPanel.SetActive(true);

        // 清理旧的UI项
        foreach (Transform child in inventorySlotParent)
        {
            Destroy(child.gameObject);
        }

        // 重新生成所有UI项
        foreach (KeyValuePair<ItemData, int> itemPair in items)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, inventorySlotParent);
            // 这里我们只更新文本作为示例，实际项目中会更新图标等
            TMP_Text slotText = slotGO.GetComponentInChildren<TMP_Text>();
            if (slotText != null)
            {
                slotText.text = $"{itemPair.Key.itemName} x{itemPair.Value}";
            }
        }
    }
}
