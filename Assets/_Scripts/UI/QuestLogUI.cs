using UnityEngine;
using TMPro;
using System.Linq;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private GameObject questLogPanel;
    [SerializeField] private TextMeshProUGUI questLogText;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<object>(GameEvents.OnQuestStateChanged, UpdateQuestLog);
        UpdateQuestLog(null);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.Unsubscribe<object>(GameEvents.OnQuestStateChanged, UpdateQuestLog);
    }

    public void UpdateQuestLog(object data)
    {
        if (QuestSystem.Instance == null || questLogText == null || questLogPanel == null) return;

        var activeQuests = QuestSystem.Instance.ActiveQuests;
        bool hasActiveQuests = activeQuests.Any();
        questLogPanel.SetActive(hasActiveQuests);

        if (hasActiveQuests)
        {
            questLogText.text = "当前任务:\n";
            foreach (var quest in activeQuests)
            {
                questLogText.text += $"- {quest.questName}\n";
            }
        }
    }
}