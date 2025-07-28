// CraftingStationController.cs
using UnityEngine;

public class CraftingStationController : MonoBehaviour
{
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // ������ڷ�Χ�ڲ����½����������ú���UI���������������
            UImanager.Instance.ToggleCraftingPanel(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // δ�����ڴ˴���ʾ������ʾ
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // δ�����ڴ˴����ؽ�����ʾ
        }
    }
}