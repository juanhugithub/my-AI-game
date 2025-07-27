// NPCController.cs (��׳���޸���)
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private DialogueData initialDialogue;
    [SerializeField] private QuestData questToOfferAfterDialogue;
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            System.Action onDialogueEndAction = () => {
                if (questToOfferAfterDialogue != null)
                {
                    // ���ؼ��޸����ڵ���ǰ�����QuestSystemʵ���Ƿ����
                    if (QuestSystem.Instance != null)
                    {
                        QuestSystem.Instance.AcceptQuest(questToOfferAfterDialogue);
                    }
                    else
                    {
                        // ��������ڣ�����һ����ȷ�Ĵ�����־
                        Debug.LogError("[NPCController] �Ի�����ʱ��QuestSystem.InstanceΪ�գ�����CoreScene�е�_CoreSystems�����Ƿ���ȷ���ز�������QuestSystem�ű���");
                    }
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