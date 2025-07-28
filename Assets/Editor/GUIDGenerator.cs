// File: Editor/GUIDGenerator.cs
using UnityEngine;
using UnityEditor;

// ��Unity����ʱ������ű��ͻ�����
[InitializeOnLoad]
public static class GUIDGenerator
{
    static GUIDGenerator()
    {
        EditorApplication.projectChanged += AutoGenerateGUID;
    }

    private static void AutoGenerateGUID()
    {
        // ������Ŀ������BaseDataSO���͵��ʲ�
        var guids = AssetDatabase.FindAssets("t:BaseDataSO");
        foreach (var assetGuid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetGuid);
            var asset = AssetDatabase.LoadAssetAtPath<BaseDataSO>(path);
            if (asset != null && string.IsNullOrEmpty(asset.guid))
            {
                // ����ʲ�û��GUID����Ϊ������һ���µ�
                asset.guid = System.Guid.NewGuid().ToString();
                EditorUtility.SetDirty(asset);
                Debug.Log($"Ϊ�ʲ� {asset.name} �������µ�GUID: {asset.guid}");
            }
        }
    }
}