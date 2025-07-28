// RecipeData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "梦境小镇/数据/制造配方")]
public class RecipeData : BaseDataSO
{
    [Tooltip("产出的成品")]
    public InventoryItemSlot resultItem;
    [Tooltip("所需的原材料列表")]
    public List<InventoryItemSlot> requiredIngredients;
    [Tooltip("额外需要的金币")]
    public long requiredGold;
}