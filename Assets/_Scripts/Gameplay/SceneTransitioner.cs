// SceneTransitioner.cs (��������֧����Ʊ����)
using UnityEngine;

public class SceneTransitioner : MonoBehaviour
{
    [Header("�����л�����")]
    [Tooltip("Ҫ���ص�Ŀ�곡������")]
    public string targetSceneName;

    [Header("�������� (��ѡ)")]
    [Tooltip("����˳�����Ҫ����Ʊ��Ʒ")]
    [SerializeField] private ItemData requiredTicket;
    [SerializeField] private int ticketCost = 1;

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // ����Ƿ�����Ʊ����
            if (requiredTicket != null)
            {
                // ����Ʊ���󣬼�鱳��
                if (InventorySystem.Instance.HasItem(requiredTicket, ticketCost))
                {
                    // ��Ʊ���㣬������Ʊ���л�����
                    InventorySystem.Instance.RemoveItem(requiredTicket, ticketCost);
                    PerformTransition();
                }
                else
                {
                    // ��Ʊ���㣬������ʾ
                    UImanager.Instance.ShowGlobalHint($"��Ҫ {requiredTicket.itemName} x{ticketCost}");
                }
            }
            else
            {
                // û����Ʊ����ֱ���л�
                PerformTransition();
            }
        }
    }

    private void PerformTransition()
    {
        string currentSceneName = gameObject.scene.name;
        SceneLoader.Instance.Transition(currentSceneName, targetSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) isPlayerInRange = true; }
    private void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) isPlayerInRange = false; }
}