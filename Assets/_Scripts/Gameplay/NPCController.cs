// NPCController.cs (健壮性修复版)
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
                    // 【关键修复】在调用前，检查QuestSystem实例是否存在
                    if (QuestSystem.Instance != null)
                    {
                        QuestSystem.Instance.AcceptQuest(questToOfferAfterDialogue);
                    }
                    else
                    {
                        // 如果不存在，给出一个明确的错误日志
                        Debug.LogError("[NPCController] 对话结束时，QuestSystem.Instance为空！请检查CoreScene中的_CoreSystems对象是否正确挂载并启用了QuestSystem脚本！");
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