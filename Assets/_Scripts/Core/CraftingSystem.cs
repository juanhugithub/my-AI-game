// CraftingSystem.cs
using UnityEngine;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; private set; }
    private void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    public bool CraftItem(RecipeData recipe)
    {
        // 1. ������Ƿ��㹻
        if (DataManager.Instance.PlayerData.Gold < recipe.requiredGold)
        {
            UImanager.Instance.ShowGlobalHint("��Ҳ��㣡");
            return false;
        }

        // 2. ���������Ʒ�����Ƿ��㹻
        foreach (var ingredient in recipe.requiredIngredients)
        {
            ItemData item = Resources.Load<ItemData>($"Items/{ingredient.itemGuid}");
            if (!InventorySystem.Instance.HasItem(item, ingredient.amount))
            {
                UImanager.Instance.ShowGlobalHint($"���ϲ���: {item.itemName}");
                return false;
            }
        }

        // 3. ���ϳ��㣬ִ�����ĺ�����
        DataManager.Instance.RemoveGold(recipe.requiredGold); // ���Ľ��
        foreach (var ingredient in recipe.requiredIngredients)
        {
            ItemData item = Resources.Load<ItemData>($"Items/{ingredient.itemGuid}");
            InventorySystem.Instance.RemoveItem(item, ingredient.amount); // ������Ʒ
        }

        // 4. ��ӳ�Ʒ
        ItemData result = Resources.Load<ItemData>($"Items/{recipe.resultItem.itemGuid}");
        InventorySystem.Instance.AddItem(result, recipe.resultItem.amount);

        UImanager.Instance.ShowGlobalHint($"�ɹ�����: {result.itemName}!");
        return true;
    }
}