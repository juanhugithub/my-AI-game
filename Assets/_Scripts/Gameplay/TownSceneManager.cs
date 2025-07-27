// TownSceneManager.cs
using UnityEngine;

public class TownSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject[] entitiesToManage; // ����Һ�����NPC�ϵ�����

    private void Start()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Subscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, HandleSceneStateChanged);
        }
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, HandleSceneStateChanged);
        }
    }

    private void HandleSceneStateChanged((string sceneName, GameEvents.SceneStateType state) sceneInfo)
    {
        // ����Ƿ���С��Ϸ����
        bool isMinigameScene = (sceneInfo.sceneName == "MiniGame_TestScene" || sceneInfo.sceneName == "WatermelonScene");
        if (!isMinigameScene) return;

        // ���С��Ϸ���������أ�����С��ʵ�壻�����ж�أ�����ʾ
        bool shouldBeActive = (sceneInfo.state == GameEvents.SceneStateType.UnloadedAdditive);

        foreach (var entity in entitiesToManage)
        {
            if (entity != null)
            {
                entity.SetActive(shouldBeActive);
            }
        }
    }
}