// CropData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCrop", menuName = "�ξ�С��/����/��������")]
public class CropData : BaseDataSO
{
    public string cropName;
    [Tooltip("����������׶�ͼƬ�������ӵ�����")]
    public Sprite[] growthSprites;
    [Tooltip("������ȫ�����������Ϸ������")]
    public float growthTimeInSeconds;
    [Tooltip("������ջ����Ʒ")]
    public ItemData harvestYield;
    [Tooltip("ÿ���ջ����������")]
    public int yieldAmount = 1;
}