// ShippingBinController.cs
using UnityEngine;
using System.Collections.Generic;

public class ShippingBinController : MonoBehaviour
{
    // �ݴ浱��Ҫ���۵���Ʒ
    public List<InventoryItemSlot> itemsToSell = new List<InventoryItemSlot>();

    // ��ҽ���ʱ����һ��UI����ҷ�����Ʒ
    public void Interact()
    {
        // UImanager.Instance.OpenShippingBinPanel(this); // δ��������һ��ר�ŵ�UI
        // ���򻯷�����Ŀǰ������ʱֱ�ӳ��۱��������пɳ��۵���Ʒ
        VendingSystem.Instance.SellAllSellableItemsFromInventory(this);
    }

    public void ClearBin()
    {
        itemsToSell.Clear();
    }
}