// ItemData.cs
using UnityEngine;

/// <summary>
/// ��Ʒ����ģ�壬ʹ��ScriptableObjectʵ�֡�
/// ������������߻���Unity�༭����ֱ�Ӵ�����������Ʒ�������д���롣
/// ʵ���ˡ��������߼����롿�ĺ��������ѧ��
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "�ξ�С��/����/��Ʒ����")] // �����ܼ�Ҫ���ṩ�����Ĳ˵�·��
public class ItemData : ScriptableObject
{
    [Header("������Ϣ")]
    public string itemName; // ��Ʒ����
    public Sprite itemIcon; // ��Ʒͼ�� (2D��Ϸʹ��Sprite)

    [Header("����")]
    [TextArea(3, 5)] // �ṩһ��������༭�Ķ����ı���
    public string description; // ��Ʒ����

    // �����ڴ���չ�������ԣ��磺
    // public ItemType itemType;
    // public int maxStackSize = 99;
    // public int price;
}
