// CraftingUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // For List

public class CraftingUI : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private RecipeData recipeToDisplay; // ��ǰֻ����һ���䷽

    [Header("UIԪ��")]
    [SerializeField] private Button craftButton;
    [SerializeField] private Button closeButton;

    [Header("�䷽��ʾ����")]
    [SerializeField] private Transform ingredientsContent; // ԭ�����б�ĸ�����
    [SerializeField] private GameObject ingredientSlotPrefab; // ������ʾ����ԭ���ϵ�Ԥ�Ƽ�
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI resultNameText;
    [SerializeField] private TextMeshProUGUI goldCostText;

    void Start()
    {
        // Ϊ��ť��Ӽ�����
        craftButton.onClick.AddListener(OnCraftButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnEnable()
    {
        // ÿ����屻����ʱ��ˢ����ʾ
        RefreshUI();
    }

    // ˢ����������������ʾ
    private void RefreshUI()
    {
        if (recipeToDisplay == null) return;

        // ����ɵ�ԭ�����б�
        foreach (Transform child in ingredientsContent)
        {
            Destroy(child.gameObject);
        }

        // �����µ�ԭ�����б�
        foreach (var ingredient in recipeToDisplay.requiredIngredients)
        {
            GameObject slotGO = Instantiate(ingredientSlotPrefab, ingredientsContent);
            ItemData itemData = AssetManager.Instance.GetAssetByGUID<ItemData>(ingredient.itemGuid);
            if (itemData != null)
            {
                // ����Ԥ�Ƽ����� Image �� TextMeshProUGUI �Ӷ���
                // slotGO.GetComponentInChildren<Image>().sprite = itemData.itemIcon;
                // slotGO.GetComponentInChildren<TextMeshProUGUI>().text = $"x {ingredient.amount}";
            }
        }

        // ���²�����Ʒ�ͽ�����ĵ���ʾ
        ItemData resultItemData = AssetManager.Instance.GetAssetByGUID<ItemData>(recipeToDisplay.resultItem.itemGuid);
        if (resultItemData != null)
        {
            // resultIcon.sprite = resultItemData.itemIcon;
            resultNameText.text = resultItemData.itemName;
        }
        if (goldCostText != null)
        {
            goldCostText.text = $"���: {recipeToDisplay.requiredGold}";
        }
    }

    private void OnCraftButtonClick()
    {
        if (CraftingSystem.Instance.CraftItem(recipeToDisplay))
        {
            // ����ɹ��������ˢ��һ��UI������ʾ���������ı仯
            RefreshUI();
        }
    }

    private void OnCloseButtonClick()
    {
        // ���ú���UImanager���ر��Լ�
        UImanager.Instance.ToggleCraftingPanel(false);
    }
}