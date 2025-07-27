// StorageUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StorageUIManager : MonoBehaviour
{
    [Header("UI����")]
    [SerializeField] private GameObject storagePanel;
    [SerializeField] private Transform playerInventoryContent;
    [SerializeField] private Transform storageContent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private StorageBoxController currentOpenBox;

    // �ɺ���UImanager�����Դ����
    public void Open(StorageBoxController targetBox)
    {
        currentOpenBox = targetBox;
        storagePanel.SetActive(true);
        RefreshUI();
    }

    public void Close()
    {
        storagePanel.SetActive(false);
        currentOpenBox = null;
        // �����˴浵
        DataManager.Instance.SaveGame();
    }

    private void RefreshUI()
    {
        // ����ɵ�UI
        foreach (Transform child in playerInventoryContent) Destroy(child.gameObject);
        foreach (Transform child in storageContent) Destroy(child.gameObject);

        // �����ұ���UI
        foreach (var itemPair in InventorySystem.Instance.Items)
        {
            CreateSlot(playerInventoryContent, itemPair.Key, itemPair.Value, true);
        }

        // ��䴢����UI
        // ע�⣺������Ҫ�޸�StorageBoxController�����������ױ�����
        foreach (var itemSlot in currentOpenBox.GetStoredItems())
        {
            ItemData itemData = Resources.Load<ItemData>($"Items/{itemSlot.itemID}");
            if (itemData != null)
            {
                CreateSlot(storageContent, itemData, itemSlot.amount, false);
            }
        }
    }

    private void CreateSlot(Transform parent, ItemData item, int amount, bool isPlayerInventory)
    {
        GameObject slotGO = Instantiate(inventorySlotPrefab, parent);
        // slotGO.GetComponentInChildren<Image>("Icon").sprite = item.itemIcon; // ����Icon�Ӷ�������Image
        // slotGO.GetComponentInChildren<TextMeshProUGUI>().text = amount.ToString();

        slotGO.GetComponent<Button>().onClick.AddListener(() => {
            OnSlotClicked(item, isPlayerInventory);
        });
    }

    private void OnSlotClicked(ItemData item, bool isFromPlayerInventory)
    {
        if (isFromPlayerInventory)
        {
            // ����ұ��� -> ������
            InventorySystem.Instance.RemoveItem(item, 1);
            currentOpenBox.AddItem(item, 1);
        }
        else
        {
            // �Ӵ����� -> ��ұ���
            if (currentOpenBox.RemoveItem(item, 1))
            {
                InventorySystem.Instance.AddItem(item, 1);
            }
        }
        RefreshUI(); // ˢ������UI����ʾ�仯
    }
}