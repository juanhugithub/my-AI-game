// CraftingSystem.cs
using UnityEngine;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; private set; }
    private void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    public bool CraftItem(RecipeData recipe)
    {
        // 1. 检查金币是否足够
        if (DataManager.Instance.PlayerData.Gold < recipe.requiredGold)
        {
            UImanager.Instance.ShowGlobalHint("金币不足！");
            return false;
        }

        // 2. 检查所有物品材料是否足够
        foreach (var ingredient in recipe.requiredIngredients)
        {
            ItemData item = Resources.Load<ItemData>($"Items/{ingredient.itemGuid}");
            if (!InventorySystem.Instance.HasItem(item, ingredient.amount))
            {
                UImanager.Instance.ShowGlobalHint($"材料不足: {item.itemName}");
                return false;
            }
        }

        // 3. 材料充足，执行消耗和制造
        DataManager.Instance.RemoveGold(recipe.requiredGold); // 消耗金币
        foreach (var ingredient in recipe.requiredIngredients)
        {
            ItemData item = Resources.Load<ItemData>($"Items/{ingredient.itemGuid}");
            InventorySystem.Instance.RemoveItem(item, ingredient.amount); // 消耗物品
        }

        // 4. 添加成品
        ItemData result = Resources.Load<ItemData>($"Items/{recipe.resultItem.itemGuid}");
        InventorySystem.Instance.AddItem(result, recipe.resultItem.amount);

        UImanager.Instance.ShowGlobalHint($"成功制造: {result.itemName}!");
        return true;
    }
}