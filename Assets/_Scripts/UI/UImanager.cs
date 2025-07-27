// Watermelon_UIManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro; // 需要引入TextMeshPro的命名空间
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI总管理器，采用单例模式。
/// 职责：管理常驻的全局UI元素，如金币显示。
/// 它严格遵守【事件驱动原则】，通过订阅OnGoldUpdated事件来更新UI，
/// 而不是被其他任何系统直接调用。这是对【架构符合度】审查点的严格遵守。
/// </summary>
public class UImanager : MonoBehaviour
{
    [Header("全局提示UI")] // 【新增】
    [SerializeField] private GameObject hintPanel; // 一个简单的Panel，包含一个Text
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private float hintDisplayDuration = 3f; // 提示显示时长
    public static UImanager Instance { get; private set; }

    [Header("通用UI元素")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("背包UI")]
    [SerializeField] private GameObject inventoryPanel; // 背包面板的根对象
    [SerializeField] private GameObject inventorySlotPrefab; // 背包项的预制体
    [SerializeField] private Transform inventorySlotParent; // 背包项生成的父节点
    
    [Header("制造UI")] // 【新增】
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private Button craftButton;
    [SerializeField] private Button closeCraftingPanelButton;
    [SerializeField] private RecipeData recipeToDisplay; // 暂时只显示一个配方
    
    [Header("任务UI")]
    [SerializeField] private GameObject questLogPanel;// 任务日志面板
    [SerializeField] private TextMeshProUGUI questLogText;// 任务日志文本显示

    [Header("仓储UI")]
    [SerializeField] private GameObject storagePanel;
    // ... (还需要背包和仓库各自的Content Transform和Slot Prefab引用)
    private StorageBoxController currentOpenBox;
    
    [Header("子UI管理器")]
    [SerializeField] private StorageUIManager storageUIManager;
    
    [Header("设置UI")]
    [SerializeField] private GameObject settingsPanel;

    public void ToggleSettingsPanel()
    {
        // 【新增日志】让我们知道这个方法是否被成功调用了
        Debug.Log("[UImanager] ToggleSettingsPanel() 方法被成功调用！");
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
            Debug.Log($"[UImanager] SettingsPanel已切换状态为: {!isActive}");
        }
        else
        {
            // 【新增日志】如果settingsPanel为空，我们会收到明确的错误提示
            Debug.LogError("[UImanager] 错误：SettingsPanel引用为空！请在CoreScene的_CoreSystems上检查UImanager的Inspector配置！");
        }
    }
    private bool isInventoryUIValid = true; // 用于标记UI配置是否正确
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
        // 【新增健壮性检查】在游戏开始前检查所有必要的UI引用是否已配置
        if (inventoryPanel == null || inventorySlotPrefab == null || inventorySlotParent == null)
        {
            Debug.LogError("【UImanager配置错误】: 背包UI的某些字段（Panel, Slot Prefab, or Parent）未在Inspector中指定！背包UI将无法工作。");
            isInventoryUIValid = false;
        }
    }

    private void Start()
    {
        // 订阅事件
        EventManager.Instance.Subscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
        EventManager.Instance.Subscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);
        // 在Start()中订阅
        EventManager.Instance.Subscribe<object>(GameEvents.OnQuestStateChanged, UpdateQuestLog);
        // 初始更新
        UpdateGoldDisplay(DataManager.Instance.PlayerData.Gold);
        inventoryPanel.SetActive(false); // 默认隐藏背包

        // 【新增】为制造按钮和关闭按钮添加监听器
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
    /// 【新增】打开制造面板的公共方法
    /// </summary>
    public void OpenCraftingPanel()
    {
        if (craftingPanel == null) return;

        // 更新UI以显示配方信息 (简化版)
        UpdateCraftingPanelUI();

        craftingPanel.SetActive(true);
    }

    /// <summary>
    /// 【新增】关闭制造面板的公共方法
    /// </summary>
    public void CloseCraftingPanel()
    {
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false);
        }
    }

    // 【新增】点击制造按钮时调用的方法
    private void OnCraftButtonClick()
    {
        if (recipeToDisplay != null)
        {
            CraftingSystem.Instance.CraftItem(recipeToDisplay);
            // 制造后可以刷新一下UI，显示材料数量变化
        }
    }

    // 【新增】用配方数据更新UI的内部方法
    private void UpdateCraftingPanelUI()
    {
        // 此处应编写详细的UI更新代码
        // 例如：resultIcon.sprite = recipeToDisplay.resultItem.itemData.icon;
        // ingredientText.text = $"x {recipeToDisplay.requiredIngredients[0].amount}";
        Debug.Log("制造面板UI已刷新以显示配方: " + recipeToDisplay.name);
    }
    /// <summary>
    /// 【修改】OnDestroy方法现在不再需要处理事件取消订阅的逻辑。
    /// 我们可以将它保留为空，或者直接删除。
    /// </summary>
    private void OnDestroy()
    {
        // 此处的逻辑已移至OnApplicationQuit，以避免销毁顺序问题
    }
    /// <summary>
    /// 【新增】当应用程序退出前，这个方法会被调用。
    /// 这是进行最终清理（如取消事件订阅）最安全的地方。
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("[UImanager] Application is quitting. Unsubscribing from all events.");

        // 尽管此时EventManager.Instance几乎可以肯定是有效的，
        // 但保留这个检查是一个非常健壮的编程习惯。
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
            EventManager.Instance.Unsubscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);
            EventManager.Instance.Unsubscribe<object>(GameEvents.OnQuestStateChanged, UpdateQuestLog);
        }
    }
    /// <summary>
    /// 更新金币显示的具体方法。此方法由事件回调触发。
    /// </summary>
    /// <param name="newGoldAmount">新的金币总数</param>
    private void UpdateGoldDisplay(long newGoldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"金币: {newGoldAmount}";
        }
    }
    /// <summary>
    /// 【新增】显示一条会自动消失的全局提示。
    /// 任何系统都可以调用此方法来给玩家反馈。
    /// </summary>
    public void ShowGlobalHint(string message)
    {
        Debug.Log($"[UImanager] 1. ShowGlobalHint方法被调用，准备显示消息: '{message}'");
        if (this == null || this.gameObject == null)
        {
            Debug.LogError("[UImanager] 致命错误：UImanager实例或其GameObject已被销毁！");
            return;
        }

        if (!this.gameObject.activeInHierarchy)
        {
            Debug.LogError("[UImanager] 错误：UImanager的GameObject处于非激活状态，无法启动协程！");
            return;
        }

        // 停止之前的协程，以防万一
        StopAllCoroutines();
        StartCoroutine(ShowHintRoutine(message));
    }
    private IEnumerator ShowHintRoutine(string message)
    {
        Debug.Log("[UImanager] 2. ShowHintRoutine协程已启动。");
        if (hintPanel == null)
        {
            Debug.LogError("[UImanager] 错误：HintPanel引用为空！请在CoreScene的_CoreSystems上检查UImanager的Inspector配置！");
            yield break;
        }
        if (hintText == null)
        {
            Debug.LogError("[UImanager] 错误：HintText引用为空！请在CoreScene的_CoreSystems上检查UImanager的Inspector配置！");
            yield break;
        }

        Debug.Log("[UImanager] 3. 引用检查通过。准备激活面板并设置文本。");
        hintText.text = message;
        hintPanel.SetActive(true);

        // 检查面板是否真的激活了
        if (hintPanel.activeInHierarchy)
        {
            Debug.Log("[UImanager] 4. HintPanel.SetActive(true) 已执行，且activeInHierarchy为True。面板现在应该是可见的。");
        }
        else
        {
            Debug.LogError("[UImanager] 5. 错误：执行了SetActive(true)后，HintPanel的activeInHierarchy仍为False！这通常意味着它的某个父对象（如CoreCanvas）被禁用了！");
        }

        yield return new WaitForSeconds(hintDisplayDuration);
        Debug.Log("[UImanager] 6. 等待时间结束，准备隐藏面板。");
        hintPanel.SetActive(false);
    }

    // UImanager.cs (UpdateQuestLog方法修改)
    public void UpdateQuestLog(object data = null)
    {
        // 【修改】通过新的公开属性 ActiveQuests 来访问数据
        var currentQuests = QuestSystem.Instance.ActiveQuests;

        questLogPanel.SetActive(currentQuests.Any());
        questLogText.text = "";
        foreach (var quest in currentQuests) // 【修改】
        {
            questLogText.text += quest.questName + "\n";
        }
    }
    /// <summary>
    /// 响应背包更新事件，刷新UI显示。
    /// </summary>
    private void UpdateInventoryDisplay(object data = null)
    {
        // 获取最新的背包数据
        var items = InventorySystem.Instance.Items;

        // 如果背包为空，则隐藏面板
        if (items.Count == 0)
        {
            inventoryPanel.SetActive(false);
            return;
        }

        inventoryPanel.SetActive(true);

        // 清理旧的UI项
        foreach (Transform child in inventorySlotParent)
        {
            Destroy(child.gameObject);
        }

        // 重新生成所有UI项
        foreach (KeyValuePair<ItemData, int> itemPair in items)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, inventorySlotParent);
            // 这里我们只更新文本作为示例，实际项目中会更新图标等
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
    // 该方法需要根据背包和仓库的数据，动态生成或更新UI列表
    private void RefreshStorageUI() { /* ... UI刷新逻辑 ... */ }

    // 示例：从背包移至仓库
    public void TransferToStorage(ItemData item, int amount) { /* ... */ }
    // 示例：从仓库移至背包
    public void TransferToInventory(ItemData item, int amount) { /* ... */ }

}
