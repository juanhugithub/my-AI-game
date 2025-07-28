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
        // ֻ������һ�������¼�
        EventManager.Instance.Subscribe<(ObjectiveType, string)>(GameEvents.OnQuestObjectiveProgress, CheckProgress);
        LoadPlayerQuests();
    }
    // �����������˶�������ʱ��������Ƿ��ǵ�ǰ�ĵ���ʵ����
    // ����ǣ��򽫾�̬ʵ����Ϊnull����ֹ�����ű����ʵ������ٵĶ���
    private void OnDestroy()
    {
        // ���޸ġ�ȷ��ȡ�����ĵ������¼�
        if (EventManager.Instance != null)
            EventManager.Instance.Unsubscribe<(ObjectiveType, string)>(GameEvents.OnQuestObjectiveProgress, CheckProgress);
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

    private void CheckProgress((ObjectiveType type, string targetID) progressInfo)
    {
        foreach (QuestData quest in activeQuests.ToList())
        {
            bool questObjectiveUpdated = false;
            foreach (var objective in quest.objectives)
            {
                // ������ͺ�ID�Ƿ�ƥ�� (ID����Ϊ�գ�����ƥ�䡰���⡱Ŀ��)
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
                // ��������Ƿ�ȫ�����
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

        // ���Ž���
        DataManager.Instance.AddGold(quest.goldReward);
        foreach (var itemSlot in quest.itemRewards)
        {
            ItemData item = Resources.Load<ItemData>($"Items/{itemSlot.itemGuid}");
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