// NPCController.cs (���ع����㲥ͨ�������¼�)
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
                // ���ؼ��޸ġ��㲥һ�����Ի������͵Ľ����¼��������ϸ�NPC��ΨһID
                if (initialDialogue.speaker != null && !string.IsNullOrEmpty(initialDialogue.speaker.guid))
                {
                    EventManager.Instance.TriggerEvent(
                        GameEvents.OnQuestObjectiveProgress,
                        (ObjectiveType.TALK, initialDialogue.speaker.guid)
                    );
                }

                // �ṩ������߼����ֲ���
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