// CraftingStationController.cs
using UnityEngine;

public class CraftingStationController : MonoBehaviour
{
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // 当玩家在范围内并按下交互键，调用核心UI管理器打开制造面板
            UImanager.Instance.ToggleCraftingPanel(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 未来可在此处显示交互提示
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 未来可在此处隐藏交互提示
        }
    }
}