// ParticleEffectManager.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ����ϵͳ��ͨ��������Ч����ع�������
/// ְ�𣺹���������ҪƵ�����ŵ�������Ч��ͨ������GameObject����������ʱ�����ܿ�����
/// </summary>
public class ParticleEffectManager : MonoBehaviour
{
    public static ParticleEffectManager Instance { get; private set; }

    [Header("��Ч������")]
    [Tooltip("��������Ҫ�ػ���������ЧԤ�Ƽ���������")]
    [SerializeField] private List<GameObject> effectPrefabs;
    [Tooltip("ÿ����Ч�ڳ��еĳ�ʼ����")]
    [SerializeField] private int initialPoolSize = 5;

    // �������ݽṹ��һ������Ч����Ϊ�����Ը���Ч�Ķ���ض���Ϊֵ���ֵ�
    private Dictionary<string, Queue<GameObject>> effectPools;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        InitializePools();
    }

    /// <summary>
    /// ��ʼ������أ�����Ϸ��ʼʱ��������Ԥ�����Ч����
    /// </summary>
    private void InitializePools()
    {
        effectPools = new Dictionary<string, Queue<GameObject>>();

        foreach (var prefab in effectPrefabs)
        {
            if (prefab == null) continue;

            Queue<GameObject> objectQueue = new Queue<GameObject>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject effectInstance = Instantiate(prefab, transform); // ��Ϊ�Ӷ��󴴽�
                effectInstance.SetActive(false);
                objectQueue.Enqueue(effectInstance);
            }
            effectPools.Add(prefab.name, objectQueue);
        }
    }

    /// <summary>
    /// �Ӷ�����л�ȡ������һ��������Ч��
    /// </summary>
    /// <param name="effectName">��ЧԤ�Ƽ���׼ȷ����</param>
    /// <param name="position">��Ч���ŵ���������</param>
    public void PlayEffect(string effectName, Vector3 position)
    {
        if (!effectPools.ContainsKey(effectName))
        {
            Debug.LogWarning($"[ParticleEffectManager] ��Ч���в�������Ϊ '{effectName}' ����Ч��");
            return;
        }

        // �Ӷ�����ȡ��һ������
        GameObject effectInstance = effectPools[effectName].Dequeue();

        // �������λ��
        effectInstance.SetActive(true);
        effectInstance.transform.position = position;

        // ��ȡ������ϵͳ�����ȷ�����ܲ���
        var particleSystem = effectInstance.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
            // ʹ��Э���ڲ�����Ϻ������
            StartCoroutine(ReturnToPoolAfterDelay(effectInstance, particleSystem.main.duration));
        }

        // �������·Żض���ĩβ���Ա��´�ʹ��
        effectPools[effectName].Enqueue(effectInstance);
    }

    /// <summary>
    /// Э�̣�������Ч��������Ϻ󣬽�����������Ϊ������״̬��
    /// </summary>
    private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }
}