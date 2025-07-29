// ParticleEffectManager.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 核心系统：通用粒子特效对象池管理器。
/// 职责：管理所有需要频繁播放的粒子特效，通过复用GameObject来避免运行时的性能开销。
/// </summary>
public class ParticleEffectManager : MonoBehaviour
{
    public static ParticleEffectManager Instance { get; private set; }

    [Header("特效池配置")]
    [Tooltip("将所有需要池化的粒子特效预制件放在这里")]
    [SerializeField] private List<GameObject> effectPrefabs;
    [Tooltip("每个特效在池中的初始数量")]
    [SerializeField] private int initialPoolSize = 5;

    // 核心数据结构：一个以特效名称为键，以该特效的对象池队列为值的字典
    private Dictionary<string, Queue<GameObject>> effectPools;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        InitializePools();
    }

    /// <summary>
    /// 初始化对象池，在游戏开始时创建所有预设的特效对象。
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
                GameObject effectInstance = Instantiate(prefab, transform); // 作为子对象创建
                effectInstance.SetActive(false);
                objectQueue.Enqueue(effectInstance);
            }
            effectPools.Add(prefab.name, objectQueue);
        }
    }

    /// <summary>
    /// 从对象池中获取并播放一个粒子特效。
    /// </summary>
    /// <param name="effectName">特效预制件的准确名称</param>
    /// <param name="position">特效播放的世界坐标</param>
    public void PlayEffect(string effectName, Vector3 position)
    {
        if (!effectPools.ContainsKey(effectName))
        {
            Debug.LogWarning($"[ParticleEffectManager] 特效池中不存在名为 '{effectName}' 的特效。");
            return;
        }

        // 从队列中取出一个对象
        GameObject effectInstance = effectPools[effectName].Dequeue();

        // 激活并设置位置
        effectInstance.SetActive(true);
        effectInstance.transform.position = position;

        // 获取其粒子系统组件，确保它能播放
        var particleSystem = effectInstance.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
            // 使用协程在播放完毕后将其回收
            StartCoroutine(ReturnToPoolAfterDelay(effectInstance, particleSystem.main.duration));
        }

        // 将其重新放回队列末尾，以备下次使用
        effectPools[effectName].Enqueue(effectInstance);
    }

    /// <summary>
    /// 协程：在粒子效果播放完毕后，将其重新设置为不激活状态。
    /// </summary>
    private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }
}