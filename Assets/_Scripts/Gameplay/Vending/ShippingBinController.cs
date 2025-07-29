// ShippingBinController.cs
using UnityEngine;
using System.Collections.Generic;

public class ShippingBinController : MonoBehaviour
{
    // 暂存当天要出售的物品
    public List<InventoryItemSlot> itemsToSell = new List<InventoryItemSlot>();

    // 玩家交互时，打开一个UI让玩家放入物品
    public void Interact()
    {
        // UImanager.Instance.OpenShippingBinPanel(this); // 未来可以做一个专门的UI
        // 【简化方案】目前，交互时直接出售背包里所有可出售的物品
        VendingSystem.Instance.SellAllSellableItemsFromInventory(this);
    }

    public void ClearBin()
    {
        itemsToSell.Clear();
    }
}