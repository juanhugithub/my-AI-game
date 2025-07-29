// SeedSelectionUI.cs (����������)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ����ѡ�����Ķ���UI��������
/// ְ���ڱ�����ʱ����̬��ʾ��ұ��������п��õ����ӣ���������ҵ�ѡ��
/// </summary>
public class SeedSelectionUI : MonoBehaviour
{
    [Header("UI����")]
    [Tooltip("���ڶ�̬�������Ӳ�λ�ĸ����� (ͨ����Scroll View�µ�Content)")]
    [SerializeField] private Transform contentParent;
    [Tooltip("�������Ӳ�λ��UIԤ�Ƽ�")]
    [SerializeField] private GameObject seedSlotPrefab;
    [Tooltip("���ڹر����İ�ť")]
    [SerializeField] private Button closeButton;

    // �ڲ�����
    private FarmPlotController currentTargetPlot;
    private List<ItemData> availableSeeds = new List<ItemData>();

    private void Start()
    {
        // Ϊ�رհ�ť��Ӽ�����
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    /// <summary>
    /// ������ѡ����塣��FarmUIManager���á�
    /// </summary>
    /// <param name="targetPlot">��ҵ�ǰ����ĵؿ�</param>
    public void Open(FarmPlotController targetPlot)
    {
        currentTargetPlot = targetPlot;
        gameObject.SetActive(true);
        RefreshPanel();
    }

    /// <summary>
    /// �ر���塣
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ˢ����壬��ʾ��ұ��������п��õ����ӡ�
    /// </summary>
    private void RefreshPanel()
    {

        // 1. ����ɵ�UI��λ
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        // ��������־��
        Debug.Log($"[SeedSelectionUI] ��ʼˢ�¡���鱳����������Ʒ...");
        var playerItems = InventorySystem.Instance.Items;
        Debug.Log($"[SeedSelectionUI] �����й��� {playerItems.Count} ����Ʒ��");
        // 2. ɸѡ����������������Ϊ�����ӡ�����Ʒ
        availableSeeds = playerItems
        .Where(itemPair => itemPair.Key.usage == ItemUsage.Seed)
        .Select(itemPair => itemPair.Key)
        .ToList();
        // ��������־��
        Debug.Log($"[SeedSelectionUI] ɸѡ���ҵ� {availableSeeds.Count} �ֿ������ӡ�");
        // 3. Ϊÿ�����õ����Ӵ���UI��λ
        foreach (var seedItem in availableSeeds)
        {
            GameObject slotGO = Instantiate(seedSlotPrefab, contentParent);

            // ���ؼ����ӡ���ȡ�����UIԪ��
            Image iconImage = slotGO.GetComponent<Image>();
            TextMeshProUGUI amountText = slotGO.GetComponentInChildren<TextMeshProUGUI>();
            Button slotButton = slotGO.GetComponent<Button>();

            if (iconImage != null)
            {
                iconImage.sprite = seedItem.itemIcon;
            }
            if (amountText != null)
            {
                amountText.text = InventorySystem.Instance.Items[seedItem].ToString();
            }
            if (slotButton != null)
            {
                // Ϊ��ť��ӵ���¼��������뵱ǰ���Ӷ�Ӧ����������
                CropData cropToPlant = AssetManager.Instance.GetAllAssetsOfType<CropData>()
                                           .FirstOrDefault(crop => crop.seedItem == seedItem);
                if (cropToPlant != null)
                {
                    slotButton.onClick.AddListener(() => OnSeedSelected(cropToPlant));
                }
            }
        }
    }

    /// <summary>
    /// �������UI�ϵ��һ������ʱ���á�
    /// </summary>
    /// <param name="selectedCrop">Ҫ��ֲ������</param>
    private void OnSeedSelected(CropData selectedCrop)
    {
        // ��鱳�����Ƿ��и����ӣ��Է���һ��
        if (InventorySystem.Instance.HasItem(selectedCrop.seedItem, 1))
        {
            // ��������
            InventorySystem.Instance.RemoveItem(selectedCrop.seedItem, 1);
            // ����ؿ�ִ����ֲ
            currentTargetPlot.Plant(selectedCrop);
        }

        // ���ؼ��޸���������ɺ󣬹ر����
        Close();
    }
}