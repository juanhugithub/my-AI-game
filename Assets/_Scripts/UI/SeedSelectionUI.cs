// SeedSelectionUI.cs (最终完整版)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 种子选择面板的独立UI控制器。
/// 职责：在被激活时，动态显示玩家背包中所有可用的种子，并处理玩家的选择。
/// </summary>
public class SeedSelectionUI : MonoBehaviour
{
    [Header("UI引用")]
    [Tooltip("用于动态生成种子槽位的父对象 (通常是Scroll View下的Content)")]
    [SerializeField] private Transform contentParent;
    [Tooltip("单个种子槽位的UI预制件")]
    [SerializeField] private GameObject seedSlotPrefab;
    [Tooltip("用于关闭面板的按钮")]
    [SerializeField] private Button closeButton;

    // 内部变量
    private FarmPlotController currentTargetPlot;
    private List<ItemData> availableSeeds = new List<ItemData>();

    private void Start()
    {
        // 为关闭按钮添加监听器
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    /// <summary>
    /// 打开种子选择面板。由FarmUIManager调用。
    /// </summary>
    /// <param name="targetPlot">玩家当前点击的地块</param>
    public void Open(FarmPlotController targetPlot)
    {
        currentTargetPlot = targetPlot;
        gameObject.SetActive(true);
        RefreshPanel();
    }

    /// <summary>
    /// 关闭面板。
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 刷新面板，显示玩家背包中所有可用的种子。
    /// </summary>
    private void RefreshPanel()
    {

        // 1. 清理旧的UI槽位
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        // 【新增日志】
        Debug.Log($"[SeedSelectionUI] 开始刷新。检查背包中所有物品...");
        var playerItems = InventorySystem.Instance.Items;
        Debug.Log($"[SeedSelectionUI] 背包中共有 {playerItems.Count} 种物品。");
        // 2. 筛选出背包中所有类型为“种子”的物品
        availableSeeds = playerItems
        .Where(itemPair => itemPair.Key.usage == ItemUsage.Seed)
        .Select(itemPair => itemPair.Key)
        .ToList();
        // 【新增日志】
        Debug.Log($"[SeedSelectionUI] 筛选后，找到 {availableSeeds.Count} 种可用种子。");
        // 3. 为每个可用的种子创建UI槽位
        foreach (var seedItem in availableSeeds)
        {
            GameObject slotGO = Instantiate(seedSlotPrefab, contentParent);

            // 【关键连接】获取并填充UI元素
            Image iconImage = slotGO.GetComponent<Image>();
            TextMeshProUGUI amountText = slotGO.GetComponentInChildren<TextMeshProUGUI>();
            Button slotButton = slotGO.GetComponent<Button>();

            if (iconImage != null)
            {
                iconImage.sprite = seedItem.itemIcon;
            }
            if (amountText != null)
            {
                amountText.text = InventorySystem.Instance.Items[seedItem].ToString();
            }
            if (slotButton != null)
            {
                // 为按钮添加点击事件，并传入当前种子对应的作物数据
                CropData cropToPlant = AssetManager.Instance.GetAllAssetsOfType<CropData>()
                                           .FirstOrDefault(crop => crop.seedItem == seedItem);
                if (cropToPlant != null)
                {
                    slotButton.onClick.AddListener(() => OnSeedSelected(cropToPlant));
                }
            }
        }
    }

    /// <summary>
    /// 当玩家在UI上点击一个种子时调用。
    /// </summary>
    /// <param name="selectedCrop">要种植的作物</param>
    private void OnSeedSelected(CropData selectedCrop)
    {
        // 检查背包中是否还有该种子（以防万一）
        if (InventorySystem.Instance.HasItem(selectedCrop.seedItem, 1))
        {
            // 消耗种子
            InventorySystem.Instance.RemoveItem(selectedCrop.seedItem, 1);
            // 命令地块执行种植
            currentTargetPlot.Plant(selectedCrop);
        }

        // 【关键修复】操作完成后，关闭面板
        Close();
    }
}