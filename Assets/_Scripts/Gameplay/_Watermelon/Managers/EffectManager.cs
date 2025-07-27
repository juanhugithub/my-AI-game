// EffectManager.cs
using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance; // ����ģʽ

    public GameObject combineEffectPrefab; // �ϳ���Ч��Ԥ����
    public int poolSize = 10;              // �صĴ�С��Ԥ�ȴ���10��

    private List<GameObject> effectPool;   // ������б�

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // ��ʼ�������
        effectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject effect = Instantiate(combineEffectPrefab, transform); // ��Ϊ�Ӷ��󴴽�
            effect.SetActive(false); // ��Ϊ�Ǽ���
            effectPool.Add(effect);
        }
    }

    /// <summary>
    /// �ӳ��л�ȡһ����Ч������
    /// </summary>
    public void PlayCombineEffect(Vector3 position)
    {
        // �����أ��ҵ�һ���Ǽ������Ч
        foreach (var effect in effectPool)
        {
            if (!effect.activeInHierarchy)
            {
                effect.transform.position = position; // ����λ��
                effect.SetActive(true);               // ������
                // ������Ҫ������ϵͳ�Լ�������Ϻ��Զ�����
                // ��ͨ��������ϵͳ����� Stop Action ������Ϊ "Disable"
                return; // �ҵ�һ���ͷ���
            }
        }

        // �������������Ч����ʹ���У���������������Կ��Ƕ�̬����
        // ����������ʱֻ��ӡһ������
        Debug.LogWarning("Effect pool is full. Consider increasing the pool size.");
    }
}
