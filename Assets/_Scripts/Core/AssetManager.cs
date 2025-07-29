// AssetManager.cs (�����GetAllAssetsOfType����)
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // ��Ҫ����LINQ�����ռ�

/// <summary>
/// ����ϵͳ�������ʲ���������
/// ��Ϸ����ʱ����������Resources�ļ����µ�BaseDataSO�ʲ���
/// ������һ����GUIDΪ�����ֵ䣬������ϵͳ��ID��ȫ��ѯ��
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
    /// ����GUID��ȡ�����ʲ���
    /// </summary>
    public T GetAssetByGUID<T>(string guid) where T : BaseDataSO
    {
        if (string.IsNullOrEmpty(guid) || !assetDatabase.ContainsKey(guid)) return null;
        return assetDatabase[guid] as T;
    }

    /// <summary>
    /// ����������ȡ���ݿ�������ָ�����͵��ʲ���
    /// ���磬���ڻ�ȡ���п���ֲ�� CropData�������п������ RecipeData��
    /// </summary>
    /// <typeparam name="T">Ҫ��ȡ���ʲ����ͣ�����̳��� BaseDataSO</typeparam>
    /// <returns>һ����������ƥ�������ʲ����б�</returns>
    public List<T> GetAllAssetsOfType<T>() where T : BaseDataSO
    {
        // ʹ��C#��LINQ���Լ��ɲ�ѯ�����ֵ������ֵ(assetDatabase.Values)��
        // ɸѡ��(OfType<T>)����ΪT���ʲ�����ת��Ϊ�б�(ToList)���ء�
        return assetDatabase.Values.OfType<T>().ToList();
    }
}