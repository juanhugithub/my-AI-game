// TownUI.cs
using UnityEngine;

/// <summary>
/// TownScene������UI�����ű���
/// ������ְ�𡿣���������Canvas�ڵ��ӳ�������ʱ����ʾ/���ء�
/// </summary>
public class TownUI : MonoBehaviour
{

    // ����������������Canvas�����ڿ�����ʾ/����
    [Header("UI����")]
    [SerializeField] private Canvas townCanvas;

    private void Awake()
    {
        // ȷ��Canvas����������
        if (townCanvas == null)
        {
            townCanvas = GetComponent<Canvas>();
            if (townCanvas == null)
            {
                Debug.LogError("[TownUI] δ�ҵ�Canvas�����δ��Inspector��ָ����");
                enabled = false; // ���ýű���ֹ��������
                return;
            }
        }
    }

    private void Start()
    {
        // �����������ĳ���״̬�仯�¼�
        if (EventManager.Instance != null)
        {
            // ʹ��Tuple��Ϊ�¼�����
            EventManager.Instance.Subscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, HandleSceneStateChanged);
        }
    }

    private void OnDestroy()
    {
        // ��������ȡ������
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, HandleSceneStateChanged);
        }
    }

    /// <summary>
    /// ������״̬�仯�¼��Ļص���
    /// </summary>
    /// <param name="sceneInfo">�����������ƺ�״̬��Ԫ��</param>
    private void HandleSceneStateChanged((string sceneName, GameEvents.SceneStateType state) sceneInfo)
    {
        string sceneName = sceneInfo.sceneName;
        GameEvents.SceneStateType state = sceneInfo.state;

        // �жϱ�����/ж�صĳ����Ƿ���С��Ϸ����
        bool isMinigameScene = (sceneName == "MiniGame_TestScene" || sceneName == "WatermelonScene");

        if (!isMinigameScene) return; // ֻ����С��Ϸ�����ı仯

        if (state == GameEvents.SceneStateType.LoadedAdditive)
        {
            // ���С��Ϸ���������Ӽ��أ����س���UI
            townCanvas.gameObject.SetActive(false);
        }
        else if (state == GameEvents.SceneStateType.UnloadedAdditive)
        {
            // ���С��Ϸ������ж�أ�������ʾ����UI
            townCanvas.gameObject.SetActive(true);
        }
    }

}
