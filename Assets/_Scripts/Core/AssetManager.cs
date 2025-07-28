// AssetManager.cs
using UnityEngine;
using System.Collections.Generic;

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

    public T GetAssetByGUID<T>(string guid) where T : BaseDataSO
    {
        if (string.IsNullOrEmpty(guid) || !assetDatabase.ContainsKey(guid)) return null;
        return assetDatabase[guid] as T;
    }
}