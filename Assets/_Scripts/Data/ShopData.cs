// ShopData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewShop", menuName = "�ξ�С��/����/�̵�����")]
public class ShopData : ScriptableObject
{
    public List<InventoryItemSlot> itemsForSale; // ʹ���������е�InventoryItemSlot������Ʒ
}