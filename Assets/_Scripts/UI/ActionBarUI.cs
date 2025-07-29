// ActionBarUI.cs (最终逻辑修复版)
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class ActionBarUI : MonoBehaviour
{
    [SerializeField] private List<Button> actionBarSlots;
    private List<ItemData> trackedItems = new List<ItemData>();
    private int selectedSlotIndex = -1;

    private void OnEnable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.Subscribe<object>(GameEvents.OnInventoryUpdated, RefreshUI);
        RefreshUI(null);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.Unsubscribe<object>(GameEvents.OnInventoryUpdated, RefreshUI);
    }

    private void Start()
    {
        for (int i = 0; i < actionBarSlots.Count; i++)
        {
            int index = i;
            actionBarSlots[i].onClick.AddListener(() => OnSlotClicked(index));
        }
        RefreshUI(null);
    }

    private void OnSlotClicked(int index)
    {
        if (index == selectedSlotIndex)
        {
            selectedSlotIndex = -1;
            HandheldItemSystem.Instance.SetHeldItem(null);
        }
        else if (index < trackedItems.Count)
        {
            selectedSlotIndex = index;
            HandheldItemSystem.Instance.SetHeldItem(trackedItems[index]);
        }

        // 【关键修复】不再调用旧的、不完整的UpdateSlotSelectionVisuals()
        // 而是直接调用RefreshUI()来完整地刷新整个快捷栏的状态
        RefreshUI(null);
    }

    public void RefreshUI(object data = null)
    {
        if (InventorySystem.Instance == null) return;

        var playerItems = InventorySystem.Instance.Items;
        trackedItems = playerItems
            .Select(itemPair => itemPair.Key)
            .Where(item => item.isTool)
            .ToList();

        for (int i = 0; i < actionBarSlots.Count; i++)
        {
            Button slotButton = actionBarSlots[i];
            Transform iconTransform = slotButton.transform.Find("Icon");
            if (iconTransform == null) continue; // 安全检查

            Image iconImage = iconTransform.GetComponent<Image>();
            TextMeshProUGUI amountText = slotButton.GetComponentInChildren<TextMeshProUGUI>();

            if (i < trackedItems.Count)
            {
                ItemData item = trackedItems[i];
                iconImage.sprite = item.itemIcon;
                iconImage.enabled = true; // 【关键修复】确保有物品时，图标总是启用的
                amountText.text = "";
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false; // 空槽位的图标是禁用的
                amountText.text = "";
            }

            // 【关键修复】将选中框的视觉更新逻辑，直接整合到刷新流程中
            slotButton.GetComponent<Image>().color = (i == selectedSlotIndex) ? Color.yellow : Color.white;
        }
    }

    // 【修改】这个方法现在已经不再需要，其逻辑已被并入RefreshUI
    // private void UpdateSlotSelectionVisuals() { ... }
}