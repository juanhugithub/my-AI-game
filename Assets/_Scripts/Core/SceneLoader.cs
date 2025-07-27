// SceneLoader.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameEvents;

/// <summary>
/// 场景加载器，采用单例模式。
/// 职责单一：管理所有场景的异步加载和卸载，并提供简单的淡入淡出过渡效果。
/// 严格遵守【Unity最佳实践】，使用异步加载避免游戏卡顿。
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private Image fadeImage; // 用于淡入淡出的黑色UI图片
    [SerializeField] private float fadeDuration = 0.5f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 【全新】健壮的场景切换方法。
    /// 它接收明确的“从哪来”和“到哪去”的指令。
    /// </summary>
    /// <param name="sceneToUnload">需要卸载的场景名</param>
    /// <param name="sceneToLoad">需要加载的场景名</param>
    public void Transition(string sceneToUnload, string sceneToLoad)
    {
        StartCoroutine(TransitionRoutine(sceneToUnload, sceneToLoad));
    }
    private IEnumerator TransitionRoutine(string sceneToUnload, string sceneToLoad)
    {
        // 1. 淡出
        yield return StartCoroutine(Fade(1f));

        // 2. 卸载旧场景 (如果提供了需要卸载的场景名)
        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            Scene scene = SceneManager.GetSceneByName(sceneToUnload);
            if (scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(sceneToUnload);
            }
        }

        // 3. 加载新场景
        yield return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        // 4. 将新加载的场景设置为活动场景（这是一个好习惯）
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));

        // 5. 触发场景状态变更事件
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneToLoad, GameEvents.SceneStateType.LoadedAdditive));

        // 6. 淡入
        yield return StartCoroutine(Fade(0f));
    }

    // 【新增】当此对象被销毁时，检查它是否是当前的单例实例。
    // 如果是，则将静态实例设为null，防止其他脚本访问到已销毁的对象。
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    /// <summary>
    /// 异步加载场景。
    /// </summary>
    /// <param name="sceneName">目标场景名</param>
    /// <param name="isAdditive">是否使用叠加模式</param>
    public void LoadSceneAsync(string sceneName, bool isAdditive = false)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, isAdditive));
    }

    /// <summary>
    /// 异步卸载场景。
    /// </summary>
    /// <param name="sceneName">要卸载的场景名</param>
    public void UnloadSceneAsync(string sceneName)
    {
        StartCoroutine(UnloadSceneRoutine(sceneName));
    }
    /// <summary>
    /// 【新增】安全地重载一个叠加场景的公共接口。
    /// </summary>
    /// <param name="sceneName">要重载的场景名称</param>
    public void ReloadSceneAsync(string sceneName)
    {
        StartCoroutine(ReloadSceneRoutine(sceneName));
    }
   
    /// <summary>
    /// 【最终加固】用于安全重载场景的内部协程。
    /// 新增了卸载前的有效性检查和活动场景转移逻辑。
    /// </summary>
    private IEnumerator ReloadSceneRoutine(string sceneName)
    {
        // 1. 统一淡出
        yield return StartCoroutine(Fade(1f));

        // 2. 卸载旧场景（增加安全检查）
        Scene sceneToUnload = SceneManager.GetSceneByName(sceneName);

        // 【关键修复A】检查场景是否有效且已加载，防止对无效场景进行操作
        if (sceneToUnload.IsValid() && sceneToUnload.isLoaded)
        {
            // 【关键修复B】在卸载前，将Active Scene的权限交给我们的CoreScene，确保安全
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("CoreScene"));

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneToUnload);
            while (asyncUnload != null && !asyncUnload.isDone) { yield return null; }
            EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, SceneStateType.UnloadedAdditive));
        }
        else
        {
            Debug.LogWarning($"[SceneLoader] 尝试重载一个无效或未加载的场景: {sceneName}。重载操作已中止。");
            // 如果卸载失败，也应该恢复UI，避免卡在黑屏
            yield return StartCoroutine(Fade(0f));
            yield break; // 中止协程
        }

        // 3. 加载新场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone) { yield return null; }
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, SceneStateType.LoadedAdditive));

        // 4. 统一淡入
        yield return StartCoroutine(Fade(0f));
    }
    private IEnumerator LoadSceneRoutine(string sceneName, bool isAdditive)
    {
        // 1. 淡出
        yield return StartCoroutine(Fade(1f));

        // 2. 异步加载场景
        LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);

        // 等待加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 3. 淡入
        yield return StartCoroutine(Fade(0f));
        // 【新增】加载完成后触发事件
        // 需要传递场景名称和状态，因此我们使用Tuple
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, GameEvents.SceneStateType.LoadedAdditive));
    }

    private IEnumerator UnloadSceneRoutine(string sceneName)
    {
        // 1. 淡出
        yield return StartCoroutine(Fade(1f));

        // 2. 异步卸载场景
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        // 等待卸载完成
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        // 3. 淡入
        yield return StartCoroutine(Fade(0f));

        // 【新增】卸载完成后触发事件
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, GameEvents.SceneStateType.UnloadedAdditive));
    }
    // 【补充】我们还需要一个只加载不卸载的初始方法
    public void LoadInitialScene(string sceneName)
    {
        StartCoroutine(InitialLoadRoutine(sceneName));
    }
    private IEnumerator InitialLoadRoutine(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, GameEvents.SceneStateType.LoadedAdditive));
    }
    private IEnumerator Fade(float targetAlpha)
    {
        fadeImage.gameObject.SetActive(true);
        float startAlpha = fadeImage.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            // 【健壮性修复】在每次访问前，都检查fadeImage是否还存在
            if (fadeImage == null)
            {
                Debug.LogWarning("Fade Image 在淡入淡出过程中丢失！");
                yield break; // 提前退出协程，防止报错
            }
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
        if (targetAlpha == 0)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
}

