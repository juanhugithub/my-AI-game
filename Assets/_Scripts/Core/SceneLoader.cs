// SceneLoader.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameEvents;

/// <summary>
/// ���������������õ���ģʽ��
/// ְ��һ���������г������첽���غ�ж�أ����ṩ�򵥵ĵ��뵭������Ч����
/// �ϸ����ء�Unity���ʵ������ʹ���첽���ر�����Ϸ���١�
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private Image fadeImage; // ���ڵ��뵭���ĺ�ɫUIͼƬ
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
    /// ��ȫ�¡���׳�ĳ����л�������
    /// ��������ȷ�ġ����������͡�����ȥ����ָ�
    /// </summary>
    /// <param name="sceneToUnload">��Ҫж�صĳ�����</param>
    /// <param name="sceneToLoad">��Ҫ���صĳ�����</param>
    public void Transition(string sceneToUnload, string sceneToLoad)
    {
        StartCoroutine(TransitionRoutine(sceneToUnload, sceneToLoad));
    }
    private IEnumerator TransitionRoutine(string sceneToUnload, string sceneToLoad)
    {
        // 1. ����
        yield return StartCoroutine(Fade(1f));

        // 2. ж�ؾɳ��� (����ṩ����Ҫж�صĳ�����)
        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            Scene scene = SceneManager.GetSceneByName(sceneToUnload);
            if (scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(sceneToUnload);
            }
        }

        // 3. �����³���
        yield return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        // 4. ���¼��صĳ�������Ϊ�����������һ����ϰ�ߣ�
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));

        // 5. ��������״̬����¼�
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneToLoad, GameEvents.SceneStateType.LoadedAdditive));

        // 6. ����
        yield return StartCoroutine(Fade(0f));
    }

    // �����������˶�������ʱ��������Ƿ��ǵ�ǰ�ĵ���ʵ����
    // ����ǣ��򽫾�̬ʵ����Ϊnull����ֹ�����ű����ʵ������ٵĶ���
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    /// <summary>
    /// �첽���س�����
    /// </summary>
    /// <param name="sceneName">Ŀ�곡����</param>
    /// <param name="isAdditive">�Ƿ�ʹ�õ���ģʽ</param>
    public void LoadSceneAsync(string sceneName, bool isAdditive = false)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, isAdditive));
    }

    /// <summary>
    /// �첽ж�س�����
    /// </summary>
    /// <param name="sceneName">Ҫж�صĳ�����</param>
    public void UnloadSceneAsync(string sceneName)
    {
        StartCoroutine(UnloadSceneRoutine(sceneName));
    }
    /// <summary>
    /// ����������ȫ������һ�����ӳ����Ĺ����ӿڡ�
    /// </summary>
    /// <param name="sceneName">Ҫ���صĳ�������</param>
    public void ReloadSceneAsync(string sceneName)
    {
        StartCoroutine(ReloadSceneRoutine(sceneName));
    }
   
    /// <summary>
    /// �����ռӹ̡����ڰ�ȫ���س������ڲ�Э�̡�
    /// ������ж��ǰ����Ч�Լ��ͻ����ת���߼���
    /// </summary>
    private IEnumerator ReloadSceneRoutine(string sceneName)
    {
        // 1. ͳһ����
        yield return StartCoroutine(Fade(1f));

        // 2. ж�ؾɳ��������Ӱ�ȫ��飩
        Scene sceneToUnload = SceneManager.GetSceneByName(sceneName);

        // ���ؼ��޸�A����鳡���Ƿ���Ч���Ѽ��أ���ֹ����Ч�������в���
        if (sceneToUnload.IsValid() && sceneToUnload.isLoaded)
        {
            // ���ؼ��޸�B����ж��ǰ����Active Scene��Ȩ�޽������ǵ�CoreScene��ȷ����ȫ
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("CoreScene"));

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneToUnload);
            while (asyncUnload != null && !asyncUnload.isDone) { yield return null; }
            EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, SceneStateType.UnloadedAdditive));
        }
        else
        {
            Debug.LogWarning($"[SceneLoader] ��������һ����Ч��δ���صĳ���: {sceneName}�����ز�������ֹ��");
            // ���ж��ʧ�ܣ�ҲӦ�ûָ�UI�����⿨�ں���
            yield return StartCoroutine(Fade(0f));
            yield break; // ��ֹЭ��
        }

        // 3. �����³���
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone) { yield return null; }
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, SceneStateType.LoadedAdditive));

        // 4. ͳһ����
        yield return StartCoroutine(Fade(0f));
    }
    private IEnumerator LoadSceneRoutine(string sceneName, bool isAdditive)
    {
        // 1. ����
        yield return StartCoroutine(Fade(1f));

        // 2. �첽���س���
        LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);

        // �ȴ��������
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 3. ����
        yield return StartCoroutine(Fade(0f));
        // ��������������ɺ󴥷��¼�
        // ��Ҫ���ݳ������ƺ�״̬���������ʹ��Tuple
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, GameEvents.SceneStateType.LoadedAdditive));
    }

    private IEnumerator UnloadSceneRoutine(string sceneName)
    {
        // 1. ����
        yield return StartCoroutine(Fade(1f));

        // 2. �첽ж�س���
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        // �ȴ�ж�����
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        // 3. ����
        yield return StartCoroutine(Fade(0f));

        // ��������ж����ɺ󴥷��¼�
        EventManager.Instance.TriggerEvent(GameEvents.OnSceneStateChanged, (sceneName, GameEvents.SceneStateType.UnloadedAdditive));
    }
    // �����䡿���ǻ���Ҫһ��ֻ���ز�ж�صĳ�ʼ����
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
            // ����׳���޸�����ÿ�η���ǰ�������fadeImage�Ƿ񻹴���
            if (fadeImage == null)
            {
                Debug.LogWarning("Fade Image �ڵ��뵭�������ж�ʧ��");
                yield break; // ��ǰ�˳�Э�̣���ֹ����
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

