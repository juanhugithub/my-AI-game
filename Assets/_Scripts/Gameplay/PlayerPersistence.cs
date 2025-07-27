// PlayerPersistence.cs
using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ʹ����Ҷ����ڼ��س���ʱ��������
        }
        else
        {
            // ����������Ѵ���һ���־û���ң������ٺ�����
            Destroy(gameObject);
        }
    }
}