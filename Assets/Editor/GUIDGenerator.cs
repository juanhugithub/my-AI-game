// File: Editor/GUIDGenerator.cs
using UnityEngine;
using UnityEditor;

// 当Unity加载时，这个脚本就会运行
[InitializeOnLoad]
public static class GUIDGenerator
{
    static GUIDGenerator()
    {
        EditorApplication.projectChanged += AutoGenerateGUID;
    }

    private static void AutoGenerateGUID()
    {
        // 查找项目中所有BaseDataSO类型的资产
        var guids = AssetDatabase.FindAssets("t:BaseDataSO");
        foreach (var assetGuid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetGuid);
            var asset = AssetDatabase.LoadAssetAtPath<BaseDataSO>(path);
            if (asset != null && string.IsNullOrEmpty(asset.guid))
            {
                // 如果资产没有GUID，则为它生成一个新的
                asset.guid = System.Guid.NewGuid().ToString();
                EditorUtility.SetDirty(asset);
                Debug.Log($"为资产 {asset.name} 生成了新的GUID: {asset.guid}");
            }
        }
    }
}