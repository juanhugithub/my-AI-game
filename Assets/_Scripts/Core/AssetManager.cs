// AssetManager.cs (已添加GetAllAssetsOfType方法)
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 需要引入LINQ命名空间

/// <summary>
/// 核心系统：中央资产管理器。
/// 游戏启动时，加载所有Resources文件夹下的BaseDataSO资产，
/// 并构建一个以GUID为键的字典，供其他系统按ID安全查询。
/// </summary>
public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance { get; private set; }
    private Dictionary<string, BaseDataSO> assetDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            BuildDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuildDatabase()
    {
        assetDatabase = new Dictionary<string, BaseDataSO>();
        var allAssets = Resources.LoadAll<BaseDataSO>("");
        foreach (var asset in allAssets)
        {
            if (asset != null && !string.IsNullOrEmpty(asset.guid) && !assetDatabase.ContainsKey(asset.guid))
            {
                assetDatabase.Add(asset.guid, asset);
            }
        }
    }

    /// <summary>
    /// 根据GUID获取单个资产。
    /// </summary>
    public T GetAssetByGUID<T>(string guid) where T : BaseDataSO
    {
        if (string.IsNullOrEmpty(guid) || !assetDatabase.ContainsKey(guid)) return null;
        return assetDatabase[guid] as T;
    }

    /// <summary>
    /// 【新增】获取数据库中所有指定类型的资产。
    /// 例如，用于获取所有可种植的 CropData，或所有可制造的 RecipeData。
    /// </summary>
    /// <typeparam name="T">要获取的资产类型，必须继承自 BaseDataSO</typeparam>
    /// <returns>一个包含所有匹配类型资产的列表</returns>
    public List<T> GetAllAssetsOfType<T>() where T : BaseDataSO
    {
        // 使用C#的LINQ语言集成查询，从字典的所有值(assetDatabase.Values)中
        // 筛选出(OfType<T>)类型为T的资产，并转换为列表(ToList)返回。
        return assetDatabase.Values.OfType<T>().ToList();
    }
}