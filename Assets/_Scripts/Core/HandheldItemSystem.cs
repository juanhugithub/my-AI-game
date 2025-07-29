// HandheldItemSystem.cs
using UnityEngine;

/// <summary>
/// ����ϵͳ���ֳ���Ʒ��������
/// ��Ϊһ��ȫ�ֵ�����������׷����ҵ�ǰ�ӿ������ѡ�е���Ʒ��ʲô��
/// </summary>
public class HandheldItemSystem : MonoBehaviour
{
    /// <summary>
    /// ȫ��Ψһ�ľ�̬ʵ���������κνű����ʡ�
    /// </summary>
    public static HandheldItemSystem Instance { get; private set; }

    /// <summary>
    /// �������ԣ��������ű�����FarmPlotController����ѯ��ǰ�ֳֵ���Ʒ��
    /// ����set��˽�еģ�ֻ���ɱ�ϵͳ�ڲ��޸ġ�
    /// </summary>
    public ItemData HeldItem { get; private set; }

    // Unity���������ڷ���������Ϸ���󱻻���ʱ����
    private void Awake()
    {
        // ��׼�ĵ���ģʽʵ�֣�ȷ��ȫ��ֻ��һ��ʵ��
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // ����������Ѵ���һ��ʵ���������ٺ�����
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���õ�ǰ�ֳֵ���Ʒ��
    /// ����������ṩ��UIϵͳ����ActionBarUI������ҵ����λʱ���õġ�
    /// </summary>
    /// <param name="item">���ѡ�е���Ʒ���������null���������ұ�Ϊ���֡�</param>
    public void SetHeldItem(ItemData item)
    {
        HeldItem = item;

        // Ϊ�˷�����ԣ������ڿ���̨��ӡ����ǰ�ֳֵ���Ʒ
        // ʹ����Ԫ�����������item����Ϊnull�����
        Debug.Log($"[Handheld] ��ǰ�ֳ�: {(item != null ? item.itemName : "����")}");
    }
}