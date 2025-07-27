// StorageBoxInteractor.cs
using UnityEngine;

// ȷ���ö�����һ���й�����StorageBoxController
[RequireComponent(typeof(StorageBoxController))]
public class StorageBoxInteractor : MonoBehaviour
{
    private StorageBoxController storageBox;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        // ��ȡ������ͬһ����Ϸ�����ϵ�StorageBoxController���
        storageBox = GetComponent<StorageBoxController>();
    }

    private void Update()
    {
        // �������Ƿ��ڷ�Χ�ڣ����Ұ����˽�����
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // ���ô���������߼��ű��е�OpenStorage����
            storageBox.OpenStorage();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �����봥�������Ƿ������
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // δ�����ڴ˴���ʾ"��E��"��UI��ʾ
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // ����뿪���������Ƿ������
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // δ�����ڴ˴�����UI��ʾ
        }
    }
}