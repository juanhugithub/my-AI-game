// CraftingUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // For List

public class CraftingUI : MonoBehaviour
{
    [Header("核心引用")]
    [SerializeField] private RecipeData recipeToDisplay; // 当前只处理一个配方

    [Header("UI元素")]
    [SerializeField] private Button craftButton;
    [SerializeField] private Button closeButton;

    [Header("配方显示区域")]
    [SerializeField] private Transform ingredientsContent; // 原材料列表的父对象
    [SerializeField] private GameObject ingredientSlotPrefab; // 用于显示单个原材料的预制件
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI resultNameText;
    [SerializeField] private TextMeshProUGUI goldCostText;

    void Start()
    {
        // 为按钮添加监听器
        craftButton.onClick.AddListener(OnCraftButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnEnable()
    {
        // 每次面板被激活时，刷新显示
        RefreshUI();
    }

    // 刷新整个制造面板的显示
    private void RefreshUI()
    {
        if (recipeToDisplay == null) return;

        // 清理旧的原材料列表
        foreach (Transform child in ingredientsContent)
        {
            Destroy(child.gameObject);
        }

        // 生成新的原材料列表
        foreach (var ingredient in recipeToDisplay.requiredIngredients)
        {
            GameObject slotGO = Instantiate(ingredientSlotPrefab, ingredientsContent);
            ItemData itemData = AssetManager.Instance.GetAssetByGUID<ItemData>(ingredient.itemGuid);
            if (itemData != null)
            {
                // 假设预制件上有 Image 和 TextMeshProUGUI 子对象
                // slotGO.GetComponentInChildren<Image>().sprite = itemData.itemIcon;
                // slotGO.GetComponentInChildren<TextMeshProUGUI>().text = $"x {ingredient.amount}";
            }
        }

        // 更新产出物品和金币消耗的显示
        ItemData resultItemData = AssetManager.Instance.GetAssetByGUID<ItemData>(recipeToDisplay.resultItem.itemGuid);
        if (resultItemData != null)
        {
            // resultIcon.sprite = resultItemData.itemIcon;
            resultNameText.text = resultItemData.itemName;
        }
        if (goldCostText != null)
        {
            goldCostText.text = $"金币: {recipeToDisplay.requiredGold}";
        }
    }

    private void OnCraftButtonClick()
    {
        if (CraftingSystem.Instance.CraftItem(recipeToDisplay))
        {
            // 制造成功后可以再刷新一次UI，以显示材料数量的变化
            RefreshUI();
        }
    }

    private void OnCloseButtonClick()
    {
        // 调用核心UImanager来关闭自己
        UImanager.Instance.ToggleCraftingPanel(false);
    }
}