// Watermelon_UIManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro; // ��Ҫ����TextMeshPro�������ռ�
using UnityEngine;
using UnityEngine.UI;
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
    
    [Header("����UI")] // ��������
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private Button craftButton;
    [SerializeField] private Button closeCraftingPanelButton;
    [SerializeField] private RecipeData recipeToDisplay; // ��ʱֻ��ʾһ���䷽
    
    [Header("����UI")]
    [SerializeField] private GameObject questLogPanel;// ������־���
    [SerializeField] private TextMeshProUGUI questLogText;// ������־�ı���ʾ

    [Header("�ִ�UI")]
    [SerializeField] private GameObject storagePanel;
    // ... (����Ҫ�����Ͳֿ���Ե�Content Transform��Slot Prefab����)
    private StorageBoxController currentOpenBox;
    
    [Header("��UI������")]
    [SerializeField] private StorageUIManager storageUIManager;
    
    [Header("����UI")]
    [SerializeField] private GameObject settingsPanel;

    public void ToggleSettingsPanel()
    {
        // ��������־��������֪����������Ƿ񱻳ɹ�������
        Debug.Log("[UImanager] ToggleSettingsPanel() �������ɹ����ã�");
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
            Debug.Log($"[UImanager] SettingsPanel���л�״̬Ϊ: {!isActive}");
        }
        else
        {
            // ��������־�����settingsPanelΪ�գ����ǻ��յ���ȷ�Ĵ�����ʾ
            Debug.LogError("[UImanager] ����SettingsPanel����Ϊ�գ�����CoreScene��_CoreSystems�ϼ��UImanager��Inspector���ã�");
        }
    }
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

        // ��������Ϊ���찴ť�͹رհ�ť��Ӽ�����
        if (craftButton != null)
        {
            craftButton.onClick.AddListener(OnCraftButtonClick);
        }
        if (closeCraftingPanelButton != null)
        {
            closeCraftingPanelButton.onClick.AddListener(CloseCraftingPanel);
        }
    }
    /// <summary>
    /// �����������������Ĺ�������
    /// </summary>
    public void OpenCraftingPanel()
    {
        if (craftingPanel == null) return;

        // ����UI����ʾ�䷽��Ϣ (�򻯰�)
        UpdateCraftingPanelUI();

        craftingPanel.SetActive(true);
    }

    /// <summary>
    /// ���������ر��������Ĺ�������
    /// </summary>
    public void CloseCraftingPanel()
    {
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false);
        }
    }

    // ��������������찴ťʱ���õķ���
    private void OnCraftButtonClick()
    {
        if (recipeToDisplay != null)
        {
            CraftingSystem.Instance.CraftItem(recipeToDisplay);
            // ��������ˢ��һ��UI����ʾ���������仯
        }
    }

    // �����������䷽���ݸ���UI���ڲ�����
    private void UpdateCraftingPanelUI()
    {
        // �˴�Ӧ��д��ϸ��UI���´���
        // ���磺resultIcon.sprite = recipeToDisplay.resultItem.itemData.icon;
        // ingredientText.text = $"x {recipeToDisplay.requiredIngredients[0].amount}";
        Debug.Log("�������UI��ˢ������ʾ�䷽: " + recipeToDisplay.name);
    }
    /// <summary>
    /// ���޸ġ�OnDestroy�������ڲ�����Ҫ�����¼�ȡ�����ĵ��߼���
    /// ���ǿ��Խ�������Ϊ�գ�����ֱ��ɾ����
    /// </summary>
    private void OnDestroy()
    {
        // �˴����߼�������OnApplicationQuit���Ա�������˳������
    }
    /// <summary>
    /// ����������Ӧ�ó����˳�ǰ����������ᱻ���á�
    /// ���ǽ�������������ȡ���¼����ģ��ȫ�ĵط���
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("[UImanager] Application is quitting. Unsubscribing from all events.");

        // ���ܴ�ʱEventManager.Instance�������Կ϶�����Ч�ģ�
        // ��������������һ���ǳ���׳�ı��ϰ�ߡ�
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
            EventManager.Instance.Unsubscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);
            EventManager.Instance.Unsubscribe<object>(GameEvents.OnQuestStateChanged, UpdateQuestLog);
        }
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
    public void OpenStoragePanel(StorageBoxController storageBox)
    {
        if (storageUIManager != null)
        {
            storageUIManager.Open(storageBox);
        }
    }
    public void CloseStoragePanel()
    {
        storagePanel.SetActive(false);
        currentOpenBox = null;
    }
    // �÷�����Ҫ���ݱ����Ͳֿ�����ݣ���̬���ɻ����UI�б�
    private void RefreshStorageUI() { /* ... UIˢ���߼� ... */ }

    // ʾ�����ӱ��������ֿ�
    public void TransferToStorage(ItemData item, int amount) { /* ... */ }
    // ʾ�����Ӳֿ���������
    public void TransferToInventory(ItemData item, int amount) { /* ... */ }

}
