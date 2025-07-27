// EconomySystem.cs
using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    private void Start()
    {
        // ȷ�� EventManager �����ٶ���
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Subscribe<int>(GameEvents.OnMiniGameFinished, HandleMiniGameFinished);
        }
    }

    private void OnDestroy()
    {
        // ȷ�� EventManager ������ȡ������
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<int>(GameEvents.OnMiniGameFinished, HandleMiniGameFinished);
        }
    }

    /// <summary>
    /// ����С��Ϸ�����¼��Ļص�������
    /// </summary>
    /// <param name="score">���¼��л�õķ���</param>
    private void HandleMiniGameFinished(int score)
    {
        // 1. ����DataManager���ӽ�ҡ�������������Ѿ����ָ��ˡ�
        DataManager.Instance.AddGold(score);

        // 2. �߼�������ɣ��㲥һ���µġ�����Ѹ��¡��¼����������µĽ������
        long newGoldTotal = DataManager.Instance.PlayerData.Gold;
        EventManager.Instance.TriggerEvent(GameEvents.OnGoldUpdated, newGoldTotal);
    }
}
