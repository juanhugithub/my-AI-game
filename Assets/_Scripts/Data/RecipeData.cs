// RecipeData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "�ξ�С��/����/�����䷽")]
public class RecipeData : BaseDataSO
{
    [Tooltip("�����ĳ�Ʒ")]
    public InventoryItemSlot resultItem;
    [Tooltip("�����ԭ�����б�")]
    public List<InventoryItemSlot> requiredIngredients;
    [Tooltip("������Ҫ�Ľ��")]
    public long requiredGold;
}