// WatermelonBridge.cs (V4 - �����߼���)
using UnityEngine;

/// <summary>
/// �������ϡ��淨�롰�ξ�С����ϵͳ������������������
/// ��V4ְ����¡��������ܼ�����ָʾ�����������֡����ò˵����͡�TopLine��Ϸ���������߼���
/// ��V4.1ְ����¡������������¿�ʼ����ť�ĳ��������߼���ʹ������Ŀ����ʽ�ܹ����ݡ�
/// </summary>
public class WatermelonBridge : MonoBehaviour
{
    public static WatermelonBridge Instance { get; private set; }

    [SerializeField] private GameManager watermelonGameManager;
    [SerializeField] private ScoreManager watermelonScoreManager;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        if (watermelonGameManager == null) watermelonGameManager = FindObjectOfType<GameManager>();
        if (watermelonScoreManager == null) watermelonScoreManager = FindObjectOfType<ScoreManager>();
    }

    // --- �����ӿ� ---

    /// <summary>
    /// ����������TopLine���������ս���ӿڡ�
    /// ���� 1:1 �һ��ɽ�ҡ�
    /// </summary>
    public void ReportFinalResult(int finalScore)
    {
        Debug.Log($"[WatermelonBridge] �յ�������Ϸ������÷�: {finalScore}������1:1�һ���ҡ�");

        // ������ϵͳ�¼���EconomySystem���Զ�����
        EventManager.Instance.TriggerEvent(GameEvents.OnMiniGameFinished, finalScore);

        // �ӳ�2�루����ҿ�һ�·�������Ȼ�󷵻�С��
        Invoke(nameof(UnloadScene), 2f);
    }

    /// <summary>
    /// ���޸ġ��������á�����еġ�����С�򡱰�ť�����
    /// ������������������һ��ɽ�ң�Ȼ�������˳���
    /// </summary>
    public void HandleReturnToTownFromSettings()
    {
        Time.timeScale = 1f; // �����Ȼָ�ʱ�䣡
        Debug.Log("[WatermelonBridge] �����ò˵����󷵻�С�򲢽���...");

        int currentScore = Mathf.FloorToInt(watermelonScoreManager.TotalScore);
        int scoreToConvert = currentScore / 2;
        Debug.Log($"��ǰ�÷�: {currentScore}, �۰��ɶһ����: {scoreToConvert}");

        EventManager.Instance.TriggerEvent(GameEvents.OnMiniGameFinished, scoreToConvert);

        UnloadScene();
    }
    
    /// <summary>
    /// ���ش��޸ġ��������á�����еġ����¿�ʼ����ť�����
    ///  ����ֻ�����SceneLoader�з�װ�õġ���ȫ�ķ������ɡ�
    /// </summary>
    public void HandleRestartFromSettings()
    {
        Time.timeScale = 1f; // �ָ�ʱ��
        // ���� SceneLoader ���½ӿڣ��������߼�ί�и���
        SceneLoader.Instance.ReloadSceneAsync("WatermelonScene");
    }
    private void UnloadScene()
    {
        SceneLoader.Instance.UnloadSceneAsync("WatermelonScene");
    }
}