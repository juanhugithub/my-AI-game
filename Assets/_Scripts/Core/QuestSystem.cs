// QuestSystem.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // For FirstOrDefault

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }

    private Dictionary<string, QuestData> questDatabase;
    private List<QuestData> activeQuests = new List<QuestData>();
    // ���������ṩһ�������ġ�ֻ���Ľӿ������ʼ���������б�
    public IReadOnlyList<QuestData> ActiveQuests => activeQuests;
    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        LoadQuestDatabase();
    }

    private void Start()
    {
        // �¼�����
        EventManager.Instance.Subscribe<object>(GameEvents.OnInventoryUpdated, (data) => CheckQuestProgress(ObjectiveType.GATHER, null));
        EventManager.Instance.Subscribe<int>(GameEvents.OnMiniGameFinished, (score) => CheckQuestProgress(ObjectiveType.PLAY_GAME, 1));

        LoadPlayerQuests();
    }

    public void AcceptQuest(QuestData quest)
    {
        if (quest == null || activeQuests.Contains(quest) || DataManager.Instance.PlayerData.completedQuests.Contains(quest.name)) return;

        activeQuests.Add(quest);
        DataManager.Instance.PlayerData.activeQuests.Add(quest.name);
        Debug.Log($"[QuestSystem] ׼��Ϊ��������'{quest.questName}'����ȫ����ʾ...");
        // ���޸ġ����ٴ����¼������ǵ���ȫ����ʾϵͳ
        UImanager.Instance.ShowGlobalHint($"�������ѽ���: {quest.questName}");
        Debug.Log($"�����ѽ���: {quest.questName}");
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
                    // �򻯴����߼���ʵ����Ŀ�������
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

        // ���Ž���
        DataManager.Instance.AddGold(quest.goldReward);
        foreach (var itemSlot in quest.itemRewards)
        {
            ItemData item = Resources.Load<ItemData>($"Items/{itemSlot.itemID}");
            if (item != null) InventorySystem.Instance.AddItem(item, itemSlot.amount);
        }
        Debug.Log($"[QuestSystem] ׼��Ϊ�������'{quest.questName}'����ȫ����ʾ...");
        // ������������ȫ����ʾϵͳ�����������ȷ����ɷ�����
        UImanager.Instance.ShowGlobalHint($"�������: {quest.questName}!");
        EventManager.Instance.TriggerEvent<object>(GameEvents.OnQuestStateChanged, null);
        Debug.Log($"���������: {quest.questName}");
    }

    // --- ���ݿ���浵���� ---
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