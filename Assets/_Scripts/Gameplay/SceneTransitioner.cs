// SceneTransitioner.cs (已升级，支持门票消耗)
using UnityEngine;

public class SceneTransitioner : MonoBehaviour
{
    [Header("场景切换设置")]
    [Tooltip("要加载的目标场景名称")]
    public string targetSceneName;

    [Header("进入条件 (可选)")]
    [Tooltip("进入此场景需要的门票物品")]
    [SerializeField] private ItemData requiredTicket;
    [SerializeField] private int ticketCost = 1;

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // 检查是否有门票需求
            if (requiredTicket != null)
            {
                // 有门票需求，检查背包
                if (InventorySystem.Instance.HasItem(requiredTicket, ticketCost))
                {
                    // 门票充足，消耗门票并切换场景
                    InventorySystem.Instance.RemoveItem(requiredTicket, ticketCost);
                    PerformTransition();
                }
                else
                {
                    // 门票不足，弹出提示
                    UImanager.Instance.ShowGlobalHint($"需要 {requiredTicket.itemName} x{ticketCost}");
                }
            }
            else
            {
                // 没有门票需求，直接切换
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