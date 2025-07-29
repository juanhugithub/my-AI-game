// ItemData.cs
using UnityEngine;

// ��������������Ʒ�ĺ�����;
public enum ItemUsage { General, Seed, Tool_WateringCan, Tool_Fertilizer }
/// <summary>
/// ��Ʒ����ģ�壬ʹ��ScriptableObjectʵ�֡�
/// ������������߻���Unity�༭����ֱ�Ӵ�����������Ʒ�������д���롣
/// ʵ���ˡ��������߼����롿�ĺ��������ѧ��
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "�ξ�С��/����/��Ʒ����")] // �����ܼ�Ҫ���ṩ�����Ĳ˵�·��
public class ItemData : BaseDataSO
{
    [Header("������Ϣ")]
    public string itemName; // ��Ʒ����
    public Sprite itemIcon; // ��Ʒͼ�� (2D��Ϸʹ��Sprite)
   
    [Header("��������;")]
    public ItemUsage usage; // ����������Ʒ��;

    [Tooltip("�������һ������(Seed)�����ڴ˴����������ֳ�����������")]
    public CropData seedForCrop; // ��������

    [Header("����")]
    [TextArea(3, 5)] // �ṩһ��������༭�Ķ����ı���
    public string description; // ��Ʒ����

    [Header("��������")]
    public int buyPrice;  // ��������������Ʒ�Ĺ���۸�
    public int sellPrice; // ���޸ġ�������Ʒ�������۸�

    [Header("��Ʒ����")]
    public bool isTool = false; // ��������

    // �����ڴ���չ�������ԣ��磺
    // public ItemType itemType;
    // public int maxStackSize = 99;
    // public int price;
}
