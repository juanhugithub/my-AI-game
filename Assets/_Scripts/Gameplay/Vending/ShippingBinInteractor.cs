// ShippingBinInteractor.cs
using UnityEngine;

/// <summary>
/// �������ר�ý�������
/// ְ��һ��ֻ��������ҵĿ����ͽ������룬
/// Ȼ����ù�����ͬһ�������ϵ�ShippingBinController�ĺ��ķ�����
/// </summary>
[RequireComponent(typeof(ShippingBinController))] // ǿ��Ҫ��ö����ϱ�����ShippingBinController
public class ShippingBinInteractor : MonoBehaviour
{
    // �Գ���������߼��ű�������
    private ShippingBinController shippingBin;
    // �������Ƿ��ڽ�����Χ��
    private bool isPlayerInRange = false;

    private void Awake()
    {
        // �ڻ���ʱ���Զ���ȡ������ͬһ����Ϸ�����ϵ�ShippingBinController���
        shippingBin = GetComponent<ShippingBinController>();
    }

    private void Update()
    {
        // ÿһ֡����飺����Ƿ��ڷ�Χ�ڣ������Ƿ����˽�����
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // ����������㣬����ó���������߼��ű��е�Interact����
            shippingBin.Interact();
        }
    }

    // ��������ײ�����˶���Ĵ�����ʱ����Unity�Զ�����
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ��������Ƿ��Ǵ���"Player"��ǩ�Ķ���
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // δ�����ڴ˴���ʾ"��E������Ʒ"��UI��ʾ
        }
    }

    // ��������ײ���뿪�˶���Ĵ�����ʱ����Unity�Զ�����
    private void OnTriggerExit2D(Collider2D other)
    {
        // ����뿪���Ƿ������
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // δ�����ڴ˴�����UI��ʾ
        }
    }
}