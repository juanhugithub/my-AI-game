// CropData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCrop", menuName = "梦境小镇/数据/作物数据")]
public class CropData : BaseDataSO
{
    public string cropName;
    [Tooltip("作物的生长阶段图片，从种子到成熟")]
    public Sprite[] growthSprites;
    [Tooltip("作物完全成熟所需的游戏内秒数")]
    public float growthTimeInSeconds;
    [Tooltip("成熟后收获的物品")]
    public ItemData harvestYield;
    [Tooltip("每次收获产出的数量")]
    public int yieldAmount = 1;
}