// TownSceneManager.cs
using UnityEngine;

public class TownSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject[] entitiesToManage; // 将玩家和所有NPC拖到这里

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
        // 检查是否是小游戏场景
        bool isMinigameScene = (sceneInfo.sceneName == "MiniGame_TestScene" || sceneInfo.sceneName == "WatermelonScene");
        if (!isMinigameScene) return;

        // 如果小游戏场景被加载，隐藏小镇实体；如果被卸载，则显示
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