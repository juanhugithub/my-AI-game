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
    [Header("����Ԥ�Ƽ�")]
    [SerializeField] private GameObject playerPrefab; // ���������������Ԥ�Ƽ�
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

        // ���޸ġ�ʹ��SceneLoader�ı�׼�ӿ������س�ʼ����
        SceneLoader.Instance.LoadInitialScene("TownScene");
    }
}
