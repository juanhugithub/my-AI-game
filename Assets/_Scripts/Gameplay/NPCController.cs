// NPCController.cs (已重构，广播通用任务事件)
using UnityEngine;
using System;

public class NPCController : MonoBehaviour
{
    [SerializeField] private DialogueData initialDialogue;
    [SerializeField] private QuestData questToOfferAfterDialogue;
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            Action onDialogueEndAction = () => {
                // 【关键修改】广播一个“对话”类型的进度事件，并附上该NPC的唯一ID
                if (initialDialogue.speaker != null && !string.IsNullOrEmpty(initialDialogue.speaker.guid))
                {
                    EventManager.Instance.TriggerEvent(
                        GameEvents.OnQuestObjectiveProgress,
                        (ObjectiveType.TALK, initialDialogue.speaker.guid)
                    );
                }

                // 提供任务的逻辑保持不变
                if (questToOfferAfterDialogue != null && QuestSystem.Instance != null)
                {
                    QuestSystem.Instance.AcceptQuest(questToOfferAfterDialogue);
                }
            };

            DialogueSystem.Instance.StartDialogue(initialDialogue, onDialogueEndAction);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { isPlayerInRange = true; }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { isPlayerInRange = false; }
    }
}