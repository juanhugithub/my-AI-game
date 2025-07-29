// ShopData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewShop", menuName = "梦境小镇/数据/商店数据")]
public class ShopData : ScriptableObject
{
    public List<InventoryItemSlot> itemsForSale; // 使用我们已有的InventoryItemSlot定义商品
}