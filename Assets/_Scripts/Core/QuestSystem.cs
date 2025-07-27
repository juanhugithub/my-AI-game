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
        EventManager.Instance.Subscribe<object>(GameEvents.OnInventoryUpdated, (data) => CheckQuestProgress(ObjectiveType.GATHER, null));
        EventManager.Instance.Subscribe<int>(GameEvents.OnMiniGameFinished, (score) => CheckQuestProgress(ObjectiveType.PLAY_GAME, 1));

        LoadPlayerQuests();
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

    private void CheckQuestProgress(ObjectiveType type, object parameter)
    {
        foreach (QuestData quest in activeQuests.ToList()) // ToList() to allow modification during iteration
        {
            bool questCompleted = true;
            foreach (var objective in quest.objectives)
            {
                if (objective.type == type)
                {
                    // 简化处理逻辑，实际项目会更复杂
                    objective.currentAmount++;
                }
                if (objective.currentAmount < objective.amount)
                {
                    questCompleted = false;
                }
            }

            if (questCompleted)
            {
                CompleteQuest(quest);
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
            ItemData item = Resources.Load<ItemData>($"Items/{itemSlot.itemID}");
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