// GameInitializer.cs
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ��Ϸ��ʼ������������CoreScene�С�
/// ְ��ȷ������ص�GameObject���������к��Ĺ��������ڳ����л�ʱ�������٣�
/// �����س�ʼ��Ϸ������TownScene�������Ǳ�֤ȫ��ϵͳ�������ڵĹؼ���
/// </summary>
public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // ȷ������ϵͳ�������л�����ʱ���ᱻ����
        DontDestroyOnLoad(gameObject);

        // ���Ӽ������ǳ�����������Ϸ
        SceneLoader.Instance.LoadSceneAsync("TownScene", true);
    }
}
