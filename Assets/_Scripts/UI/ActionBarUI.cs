// ActionBarUI.cs (�����߼��޸���)
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

        // ���ؼ��޸������ٵ��þɵġ���������UpdateSlotSelectionVisuals()
        // ����ֱ�ӵ���RefreshUI()��������ˢ�������������״̬
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
            if (iconTransform == null) continue; // ��ȫ���

            Image iconImage = iconTransform.GetComponent<Image>();
            TextMeshProUGUI amountText = slotButton.GetComponentInChildren<TextMeshProUGUI>();

            if (i < trackedItems.Count)
            {
                ItemData item = trackedItems[i];
                iconImage.sprite = item.itemIcon;
                iconImage.enabled = true; // ���ؼ��޸���ȷ������Ʒʱ��ͼ���������õ�
                amountText.text = "";
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false; // �ղ�λ��ͼ���ǽ��õ�
                amountText.text = "";
            }

            // ���ؼ��޸�����ѡ�п���Ӿ������߼���ֱ�����ϵ�ˢ��������
            slotButton.GetComponent<Image>().color = (i == selectedSlotIndex) ? Color.yellow : Color.white;
        }
    }

    // ���޸ġ�������������Ѿ�������Ҫ�����߼��ѱ�����RefreshUI
    // private void UpdateSlotSelectionVisuals() { ... }
}