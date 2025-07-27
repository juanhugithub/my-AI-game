// GameInitializer.cs
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏初始化器，放置在CoreScene中。
/// 职责：确保其挂载的GameObject（包含所有核心管理器）在场景切换时不被销毁，
/// 并加载初始游戏场景（TownScene）。这是保证全局系统持续存在的关键。
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("核心预制件")]
    [SerializeField] private GameObject playerPrefab; // 【新增】引用玩家预制件
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (playerPrefab != null)
        {
            if (PlayerPersistence.Instance == null)
            {
                Instantiate(playerPrefab);
            }
        }

        // 【修改】使用SceneLoader的标准接口来加载初始场景
        SceneLoader.Instance.LoadInitialScene("TownScene");
    }
}
