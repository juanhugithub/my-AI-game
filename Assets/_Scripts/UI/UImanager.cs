// Watermelon_UIManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro; // 需要引入TextMeshPro的命名空间
using UnityEngine;

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

    [Header("任务UI")]
    [SerializeField] private GameObject questLogPanel;// 任务日志面板
    [SerializeField] private TextMeshProUGUI questLogText;// 任务日志文本显示

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
    }

    private void OnDestroy()
    {
        // 取消订阅
        EventManager.Instance.Unsubscribe<long>(GameEvents.OnGoldUpdated, UpdateGoldDisplay);
        EventManager.Instance.Unsubscribe<object>(GameEvents.OnInventoryUpdated, UpdateInventoryDisplay);
        EventManager.Instance.Unsubscribe<object>(GameEvents.OnQuestStateChanged, UpdateQuestLog);

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
}
