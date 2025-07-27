// MiniGameUI.cs
using UnityEngine;

/// <summary>
/// MiniGame_TestScene������UI�����ű���
/// </summary>
public class MiniGameUI : MonoBehaviour
{
    // ��Unity�༭������ק��ֵ
    [Header("��������")]
    [SerializeField] private ItemData rewardItem;

    public void OnScoreButtonClick()
    {
        // ����Ƿ������˽�����Ʒ
        if (rewardItem != null)
        {
            // ֱ�ӵ��ñ���ϵͳ�����Ʒ�������ǹ㲥�����¼�
            InventorySystem.Instance.AddItem(rewardItem, 1);
        }
        else
        {
            Debug.LogError("δ��MiniGameUI�����ý�����Ʒ��");
        }
    }

    public void OnReturnButtonClick()
    {
        // ����SceneLoaderж��������
        SceneLoader.Instance.UnloadSceneAsync("MiniGame_TestScene");
    }
}
