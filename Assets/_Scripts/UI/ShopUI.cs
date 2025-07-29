// ShopUI.cs
using UnityEngine;
// ... (需要引入UnityEngine.UI, TMPro等)

public class ShopUI : MonoBehaviour
{
    // ... (此处包含UI元素引用，如商品列表的Content, 商品槽位Prefab)

    public void OpenShop(ShopData data)
    {
        gameObject.SetActive(true);
        PopulateShop(data);
    }

    private void PopulateShop(ShopData data)
    {
        // 1. 清空旧的商品列表
        // 2. 遍历data.itemsForSale
        // 3. 为每个商品实例化一个Slot Prefab
        // 4. 填充Prefab的图标、名称、价格
        // 5. 为Prefab的购买按钮添加监听器: () => ShopSystem.Instance.PurchaseItem(...)
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
        UImanager.Instance.CloseShopPanel(); // 通知UImanager恢复玩家状态
    }
}