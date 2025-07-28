// QuestSystem.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // For FirstOrDefault

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }

    private Dictionary<string, QuestData> questDatabase;
    private List<QuestData> activeQuests = new List<QuestData>();
    // 【新增】提供一个公开的、只读的接口来访问激活的任务列表
    public IReadOnlyList<QuestData> ActiveQuests => activeQuests;
    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        LoadQuestDatabase();
    }

    private void Start()
    {
        // 事件订阅
        // 只订阅这一个核心事件
        EventManager.Instance.Subscribe<(ObjectiveType, string)>(GameEvents.OnQuestObjectiveProgress, CheckProgress);
        LoadPlayerQuests();
    }
    // 【新增】当此对象被销毁时，检查它是否是当前的单例实例。
    // 如果是，则将静态实例设为null，防止其他脚本访问到已销毁的对象。
    private void OnDestroy()
    {
        // 【修改】确保取消订阅的是新事件
        if (EventManager.Instance != null)
            EventManager.Instance.Unsubscribe<(ObjectiveType, string)>(GameEvents.OnQuestObjectiveProgress, CheckProgress);
    }
    public void AcceptQuest(QuestData quest)
    {
        if (quest == null || activeQuests.Contains(quest) || DataManager.Instance.PlayerData.completedQuests.Contains(quest.name)) return;

        activeQuests.Add(quest);
        DataManager.Instance.PlayerData.activeQuests.Add(quest.name);
        Debug.Log($"[QuestSystem] 准备为接受任务'{quest.questName}'调用全局提示...");
        // 【修改】不再触发事件，而是调用全局提示系统
        UImanager.Instance.ShowGlobalHint($"新任务已接受: {quest.questName}");
        Debug.Log($"任务已接受: {quest.questName}");
    }

    private void CheckProgress((ObjectiveType type, string targetID) progressInfo)
    {
        foreach (QuestData quest in activeQuests.ToList())
        {
            bool questObjectiveUpdated = false;
            foreach (var objective in quest.objectives)
            {
                // 检查类型和ID是否匹配 (ID可以为空，用于匹配“任意”目标)
                if (objective.type == progressInfo.type && (string.IsNullOrEmpty(objective.targetID) || objective.targetID == progressInfo.targetID))
                {
                    if (objective.currentAmount < objective.amount)
                    {
                        objective.currentAmount++;
                        questObjectiveUpdated = true;
                    }
                }
            }

            if (questObjectiveUpdated)
            {
                // 检查任务是否全部完成
                if (quest.objectives.All(obj => obj.currentAmount >= obj.amount))
                {
                    CompleteQuest(quest);
                }
            }
        }
    }

    private void CompleteQuest(QuestData quest)
    {
        activeQuests.Remove(quest);
        DataManager.Instance.PlayerData.activeQuests.Remove(quest.name);
        DataManager.Instance.PlayerData.completedQuests.Add(quest.name);

        // 发放奖励
        DataManager.Instance.AddGold(quest.goldReward);
        foreach (var itemSlot in quest.itemRewards)
        {
            ItemData item = Resources.Load<ItemData>($"Items/{itemSlot.itemGuid}");
            if (item != null) InventorySystem.Instance.AddItem(item, itemSlot.amount);
        }
        Debug.Log($"[QuestSystem] 准备为完成任务'{quest.questName}'调用全局提示...");
        // 【新增】调用全局提示系统，给予玩家明确的完成反馈！
        UImanager.Instance.ShowGlobalHint($"任务完成: {quest.questName}!");
        EventManager.Instance.TriggerEvent<object>(GameEvents.OnQuestStateChanged, null);
        Debug.Log($"任务已完成: {quest.questName}");
    }

    // --- 数据库与存档加载 ---
    private void LoadQuestDatabase()
    {
        questDatabase = new Dictionary<string, QuestData>();
        var allQuests = Resources.LoadAll<QuestData>("Quests");
        foreach (var quest in allQuests)
        {
            questDatabase[quest.name] = quest;
        }
    }

    private void LoadPlayerQuests()
    {
        var pd = DataManager.Instance.PlayerData;
        activeQuests.Clear();
        foreach (string questName in pd.activeQuests)
        {
            if (questDatabase.TryGetValue(questName, out QuestData quest))
            {
                activeQuests.Add(quest);
            }
        }
    }
}