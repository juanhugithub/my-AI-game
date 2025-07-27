// Watermelon_UIManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro; // ��Ҫ����TextMeshPro�������ռ�
using UnityEngine;

/// <summary>
/// UI�ܹ����������õ���ģʽ��
/// ְ�𣺹���פ��ȫ��UIԪ�أ�������ʾ��
/// ���ϸ����ء��¼�����ԭ�򡿣�ͨ������OnGoldUpdated�¼�������UI��
/// �����Ǳ������κ�ϵͳֱ�ӵ��á����Ƕԡ��ܹ����϶ȡ�������ϸ����ء�
/// </summary>
public class UImanager : MonoBehaviour
{
    [Header("ȫ����ʾUI")] // ��������
    [SerializeField] private GameObject hintPanel; // һ���򵥵�Panel������һ��Text
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private float hintDisplayDuration = 3f; // ��ʾ��ʾʱ��
    public static UImanager Instance { get; private set; }

    [Header("ͨ��UIԪ��")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("����UI")]
    [SerializeField] private GameObject inventoryPanel; // �������ĸ�����
    [SerializeField] private GameObject inventorySlotPrefab; // �������Ԥ����
    [SerializeField] private Transform inventorySlotParent; // ���������ɵĸ��ڵ�

    [Header("����UI")]
    [SerializeField] private GameObject questLogPanel;// ������־���
    [SerializeField] private TextMeshProUGUI questLogText;// ������־�ı���ʾ

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
        // ��Start()�ж���
        EventManager.Instance.Subscribe<object>(GameEvents.OnQuestStateChanged, UpdateQuestLog);
        // ��ʼ����
        UpdateGoldDisplay(DataManager.Instance.PlayerData.Gold);
        inventoryPanel.SetActive(false); // Ĭ�����ر���
    }

    private void OnDestroy()
    {
        // ȡ������
        EventManager.Instance.Unsubscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
        EventManager.Instance.Unsubscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);
        EventManager.Instance.Unsubscribe<object>(GameEvents.OnQuestStateChanged, UpdateQuestLog);

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
    /// ����������ʾһ�����Զ���ʧ��ȫ����ʾ��
    /// �κ�ϵͳ�����Ե��ô˷���������ҷ�����
    /// </summary>
    public void ShowGlobalHint(string message)
    {
        Debug.Log($"[UImanager] 1. ShowGlobalHint���������ã�׼����ʾ��Ϣ: '{message}'");
        if (this == null || this.gameObject == null)
        {
            Debug.LogError("[UImanager] ��������UImanagerʵ������GameObject�ѱ����٣�");
            return;
        }

        if (!this.gameObject.activeInHierarchy)
        {
            Debug.LogError("[UImanager] ����UImanager��GameObject���ڷǼ���״̬���޷�����Э�̣�");
            return;
        }

        // ֹ֮ͣǰ��Э�̣��Է���һ
        StopAllCoroutines();
        StartCoroutine(ShowHintRoutine(message));
    }
    private IEnumerator ShowHintRoutine(string message)
    {
        Debug.Log("[UImanager] 2. ShowHintRoutineЭ����������");
        if (hintPanel == null)
        {
            Debug.LogError("[UImanager] ����HintPanel����Ϊ�գ�����CoreScene��_CoreSystems�ϼ��UImanager��Inspector���ã�");
            yield break;
        }
        if (hintText == null)
        {
            Debug.LogError("[UImanager] ����HintText����Ϊ�գ�����CoreScene��_CoreSystems�ϼ��UImanager��Inspector���ã�");
            yield break;
        }

        Debug.Log("[UImanager] 3. ���ü��ͨ����׼��������岢�����ı���");
        hintText.text = message;
        hintPanel.SetActive(true);

        // �������Ƿ���ļ�����
        if (hintPanel.activeInHierarchy)
        {
            Debug.Log("[UImanager] 4. HintPanel.SetActive(true) ��ִ�У���activeInHierarchyΪTrue���������Ӧ���ǿɼ��ġ�");
        }
        else
        {
            Debug.LogError("[UImanager] 5. ����ִ����SetActive(true)��HintPanel��activeInHierarchy��ΪFalse����ͨ����ζ������ĳ����������CoreCanvas���������ˣ�");
        }

        yield return new WaitForSeconds(hintDisplayDuration);
        Debug.Log("[UImanager] 6. �ȴ�ʱ�������׼��������塣");
        hintPanel.SetActive(false);
    }

    // UImanager.cs (UpdateQuestLog�����޸�)
    public void UpdateQuestLog(object data = null)
    {
        // ���޸ġ�ͨ���µĹ������� ActiveQuests ����������
        var currentQuests = QuestSystem.Instance.ActiveQuests;

        questLogPanel.SetActive(currentQuests.Any());
        questLogText.text = "";
        foreach (var quest in currentQuests) // ���޸ġ�
        {
            questLogText.text += quest.questName + "\n";
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
