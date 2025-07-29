// CropData.cs (已升级以支持农场系统2.0)
using UnityEngine;

// 【注意】请确保您的CropData.cs文件继承的是BaseDataSO，以兼容我们的GUID系统
[CreateAssetMenu(fileName = "NewCrop", menuName = "梦境小镇/数据/作物数据")]
public class CropData : BaseDataSO
{
    // 【关键】这个公开的字符串变量，正是编译器报错说找不到的定义
    public string cropName;

    [Tooltip("作物的生长阶段图片，0=种子, 1=浇水后, 2...N-1=生长中, N=成熟")]
    public Sprite[] growthSprites;

    [Tooltip("作物完全成熟所需的游戏内天数")]
    public int daysToGrow;
   
     [Header("种植/产出")]
    public ItemData seedItem; // 种植此作物所需的种子物品
    public ItemData harvestYield;
    [Tooltip("最小产出数量")]
    public int minYieldAmount = 1; // 【新增】
    [Tooltip("最大产出数量（包含）")]
    public int maxYieldAmount = 9; // 【新增】

    
}