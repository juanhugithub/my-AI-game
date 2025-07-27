// Watermelon_UIManager.cs
using TMPro; // ��Ҫ����TextMeshPro�������ռ�
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI�ܹ����������õ���ģʽ��
/// ְ�𣺹���פ��ȫ��UIԪ�أ�������ʾ��
/// ���ϸ����ء��¼�����ԭ�򡿣�ͨ������OnGoldUpdated�¼�������UI��
/// �����Ǳ������κ�ϵͳֱ�ӵ��á����Ƕԡ��ܹ����϶ȡ�������ϸ����ء�
/// </summary>
public class UImanager : MonoBehaviour
{
    public static UImanager Instance { get; private set; }

    [Header("ͨ��UIԪ��")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("����UI")]
    [SerializeField] private GameObject inventoryPanel; // �������ĸ�����
    [SerializeField] private GameObject inventorySlotPrefab; // �������Ԥ����
    [SerializeField] private Transform inventorySlotParent; // ���������ɵĸ��ڵ�

    private bool isInventoryUIValid = true; // ���ڱ��UI�����Ƿ���ȷ
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // ��������׳�Լ�顿����Ϸ��ʼǰ������б�Ҫ��UI�����Ƿ�������
        if (inventoryPanel == null || inventorySlotPrefab == null || inventorySlotParent == null)
        {
            Debug.LogError("��UImanager���ô���: ����UI��ĳЩ�ֶΣ�Panel, Slot Prefab, or Parent��δ��Inspector��ָ��������UI���޷�������");
            isInventoryUIValid = false;
        }
    }

    private void Start()
    {
        // �����¼�
        EventManager.Instance.Subscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
        EventManager.Instance.Subscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);

        // ��ʼ����
        UpdateGoldDisplay(DataManager.Instance.PlayerData.Gold);
        inventoryPanel.SetActive(false); // Ĭ�����ر���
    }

    private void OnDestroy()
    {
        // ȡ������
        EventManager.Instance.Unsubscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
        EventManager.Instance.Unsubscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);
    }

    /// <summary>
    /// ���½����ʾ�ľ��巽�����˷������¼��ص�������
    /// </summary>
    /// <param name="newGoldAmount">�µĽ������</param>
    private void UpdateGoldDisplay(long newGoldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"���: {newGoldAmount}";
        }
    }

    /// <summary>
    /// ��Ӧ���������¼���ˢ��UI��ʾ��
    /// </summary>
    private void UpdateInventoryDisplay(object data = null)
    {
        // ��ȡ���µı�������
        var items = InventorySystem.Instance.Items;

        // �������Ϊ�գ����������
        if (items.Count == 0)
        {
            inventoryPanel.SetActive(false);
            return;
        }

        inventoryPanel.SetActive(true);

        // ����ɵ�UI��
        foreach (Transform child in inventorySlotParent)
        {
            Destroy(child.gameObject);
        }

        // ������������UI��
        foreach (KeyValuePair<ItemData, int> itemPair in items)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, inventorySlotParent);
            // ��������ֻ�����ı���Ϊʾ����ʵ����Ŀ�л����ͼ���
            TMP_Text slotText = slotGO.GetComponentInChildren<TMP_Text>();
            if (slotText != null)
            {
                slotText.text = $"{itemPair.Key.itemName} x{itemPair.Value}";
            }
        }
    }
}
