// ShopUI.cs
using UnityEngine;
// ... (��Ҫ����UnityEngine.UI, TMPro��)

public class ShopUI : MonoBehaviour
{
    // ... (�˴�����UIԪ�����ã�����Ʒ�б��Content, ��Ʒ��λPrefab)

    public void OpenShop(ShopData data)
    {
        gameObject.SetActive(true);
        PopulateShop(data);
    }

    private void PopulateShop(ShopData data)
    {
        // 1. ��վɵ���Ʒ�б�
        // 2. ����data.itemsForSale
        // 3. Ϊÿ����Ʒʵ����һ��Slot Prefab
        // 4. ���Prefab��ͼ�ꡢ���ơ��۸�
        // 5. ΪPrefab�Ĺ���ť��Ӽ�����: () => ShopSystem.Instance.PurchaseItem(...)
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
        UImanager.Instance.CloseShopPanel(); // ֪ͨUImanager�ָ����״̬
    }
}