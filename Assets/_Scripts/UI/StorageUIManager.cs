// StorageUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StorageUIManager : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private GameObject storagePanel;
    [SerializeField] private Transform playerInventoryContent;
    [SerializeField] private Transform storageContent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private StorageBoxController currentOpenBox;

    // 由核心UImanager调用以打开面板
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
        // 别忘了存档
        DataManager.Instance.SaveGame();
    }

    private void RefreshUI()
    {
        // 清理旧的UI
        foreach (Transform child in playerInventoryContent) Destroy(child.gameObject);
        foreach (Transform child in storageContent) Destroy(child.gameObject);

        // 填充玩家背包UI
        foreach (var itemPair in InventorySystem.Instance.Items)
        {
            CreateSlot(playerInventoryContent, itemPair.Key, itemPair.Value, true);
        }

        // 填充储物箱UI
        // 注意：这里需要修改StorageBoxController来让它更容易被访问
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
        // slotGO.GetComponentInChildren<Image>("Icon").sprite = item.itemIcon; // 假设Icon子对象上有Image
        // slotGO.GetComponentInChildren<TextMeshProUGUI>().text = amount.ToString();

        slotGO.GetComponent<Button>().onClick.AddListener(() => {
            OnSlotClicked(item, isPlayerInventory);
        });
    }

    private void OnSlotClicked(ItemData item, bool isFromPlayerInventory)
    {
        if (isFromPlayerInventory)
        {
            // 从玩家背包 -> 储物箱
            InventorySystem.Instance.RemoveItem(item, 1);
            currentOpenBox.AddItem(item, 1);
        }
        else
        {
            // 从储物箱 -> 玩家背包
            if (currentOpenBox.RemoveItem(item, 1))
            {
                InventorySystem.Instance.AddItem(item, 1);
            }
        }
        RefreshUI(); // 刷新整个UI以显示变化
    }
}