// EffectManager.cs
using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance; // 单例模式

    public GameObject combineEffectPrefab; // 合成特效的预制体
    public int poolSize = 10;              // 池的大小，预先创建10个

    private List<GameObject> effectPool;   // 对象池列表

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 初始化对象池
        effectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject effect = Instantiate(combineEffectPrefab, transform); // 作为子对象创建
            effect.SetActive(false); // 设为非激活
            effectPool.Add(effect);
        }
    }

    /// <summary>
    /// 从池中获取一个特效并播放
    /// </summary>
    public void PlayCombineEffect(Vector3 position)
    {
        // 遍历池，找到一个非激活的特效
        foreach (var effect in effectPool)
        {
            if (!effect.activeInHierarchy)
            {
                effect.transform.position = position; // 设置位置
                effect.SetActive(true);               // 激活它
                // 我们需要让粒子系统自己播放完毕后自动禁用
                // 这通常在粒子系统组件的 Stop Action 里设置为 "Disable"
                return; // 找到一个就返回
            }
        }

        // 如果池中所有特效都在使用中（极端情况），可以考虑动态扩容
        // 这里我们暂时只打印一个警告
        Debug.LogWarning("Effect pool is full. Consider increasing the pool size.");
    }
}
