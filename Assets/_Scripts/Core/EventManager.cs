// EventManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局事件中心，采用单例模式。
/// 它是整个事件驱动架构的灵魂，负责所有系统间的解耦通信。
/// 系统之间不直接引用，而是通过订阅和触发事件来交互，严格遵守【高内聚，低耦合】原则。
/// </summary>
public class EventManager : MonoBehaviour
{
    // 单例实例
    public static EventManager Instance { get; private set; }

    // 用于存储所有事件的字典。键是事件名(string)，值是对应的委托(Delegate)。
    private Dictionary<string, Delegate> eventDictionary;

    private void Awake()
    {
        // 标准单例模式实现
        if (Instance == null)
        {
            Instance = this;
            eventDictionary = new Dictionary<string, Delegate>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 订阅一个带泛型参数的事件。
    /// </summary>
    /// <param name="eventName">事件名称，推荐使用GameEvents类的常量</param>
    /// <param name="listener">事件监听器（回调函数）</param>
    /// <typeparam name="T">事件参数类型</typeparam>
    public void Subscribe<T>(string eventName, Action<T> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate d))
        {
            eventDictionary[eventName] = Delegate.Combine(d, listener);
        }
        else
        {
            eventDictionary[eventName] = listener;
        }
    }

    /// <summary>
    /// 取消订阅一个带泛型参数的事件。
    /// 【已根据总监审查建议优化】：增加了对字典键存在的检查，使代码更健壮。
    /// </summary>
    public void Unsubscribe<T>(string eventName, Action<T> listener)
    {
        // 检查事件字典中是否存在该事件的委托
        if (eventDictionary.TryGetValue(eventName, out Delegate d))
        {
            Delegate currentDelegate = Delegate.Remove(d, listener);
            if (currentDelegate == null)
            {
                // 如果移除后委托为空，则从字典中移除该事件键
                eventDictionary.Remove(eventName);
            }
            else
            {
                eventDictionary[eventName] = currentDelegate;
            }
        }
    }

    /// <summary>
    /// 触发一个带泛型参数的事件。
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="eventData">事件参数</param>
    /// <typeparam name="T">事件参数类型</typeparam>
    public void TriggerEvent<T>(string eventName, T eventData)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate d))
        {
            (d as Action<T>)?.Invoke(eventData);
        }
    }

}
