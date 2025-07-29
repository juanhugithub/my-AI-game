// CropData.cs (��������֧��ũ��ϵͳ2.0)
using UnityEngine;

// ��ע�⡿��ȷ������CropData.cs�ļ��̳е���BaseDataSO���Լ������ǵ�GUIDϵͳ
[CreateAssetMenu(fileName = "NewCrop", menuName = "�ξ�С��/����/��������")]
public class CropData : BaseDataSO
{
    // ���ؼ�������������ַ������������Ǳ���������˵�Ҳ����Ķ���
    public string cropName;

    [Tooltip("����������׶�ͼƬ��0=����, 1=��ˮ��, 2...N-1=������, N=����")]
    public Sprite[] growthSprites;

    [Tooltip("������ȫ�����������Ϸ������")]
    public int daysToGrow;
   
     [Header("��ֲ/����")]
    public ItemData seedItem; // ��ֲ�����������������Ʒ
    public ItemData harvestYield;
    [Tooltip("��С��������")]
    public int minYieldAmount = 1; // ��������
    [Tooltip("������������������")]
    public int maxYieldAmount = 9; // ��������

    
}