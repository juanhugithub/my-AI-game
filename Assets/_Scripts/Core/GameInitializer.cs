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
    void Start()
    {
        // 确保核心系统对象在切换场景时不会被销毁
        DontDestroyOnLoad(gameObject);

        // 叠加加载主城场景，启动游戏
        SceneLoader.Instance.LoadSceneAsync("TownScene", true);
    }
}
