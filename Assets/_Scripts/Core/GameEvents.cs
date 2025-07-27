// GameEvents.cs

/// <summary>
/// ��̬�࣬����ͳһ������Ϸ�����е��¼������ַ�����
/// ʹ�ô���ĳ�������ֱ��д���ַ�����"ħ���ַ���"����
/// ������Ч������ƴд�����µ��¼��޷��������⣬����ߴ���Ŀ�ά���ԡ�
/// ���Ǽܹ��С�����չ����δ������������ĺ���ʵ����
/// </summary>
public static class GameEvents
{
    // ��С��Ϸ��ɲ���������ʱ����
    // ����: int (����)
    public const string OnMiniGameFinished = "OnMiniGameFinished";

    // ����ҽ���������º󴥷�
    // ����: long (�µĽ������)
    public const string OnGoldUpdated = "OnGoldUpdated";

    // ���������ݷ����仯ʱ����
    // �޲���������Ϊ֪ͨ��UI�յ����������InventorySystem��ѯ��������
    public const string OnInventoryUpdated = "OnInventoryUpdated";

    /// <summary>
    /// ��һ�����������Ӽ��ػ�ж��ʱ������
    /// ����: string (��������), SceneStateType (����/ж��״̬)
    /// </summary>
    public const string OnSceneStateChanged = "OnSceneStateChanged";
    /// <summary>
    /// ����״̬����ö�٣�����ָʾ�������ػ�ж�ص����͡�
    /// </summary>
    public enum SceneStateType { LoadedAdditive, UnloadedAdditive }
}


