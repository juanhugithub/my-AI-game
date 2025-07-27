// EventManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ȫ���¼����ģ����õ���ģʽ��
/// ���������¼������ܹ�����꣬��������ϵͳ��Ľ���ͨ�š�
/// ϵͳ֮�䲻ֱ�����ã�����ͨ�����ĺʹ����¼����������ϸ����ء����ھۣ�����ϡ�ԭ��
/// </summary>
public class EventManager : MonoBehaviour
{
    // ����ʵ��
    public static EventManager Instance { get; private set; }

    // ���ڴ洢�����¼����ֵ䡣�����¼���(string)��ֵ�Ƕ�Ӧ��ί��(Delegate)��
    private Dictionary<string, Delegate> eventDictionary;

    private void Awake()
    {
        // ��׼����ģʽʵ��
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
    /// ����һ�������Ͳ������¼���
    /// </summary>
    /// <param name="eventName">�¼����ƣ��Ƽ�ʹ��GameEvents��ĳ���</param>
    /// <param name="listener">�¼����������ص�������</param>
    /// <typeparam name="T">�¼���������</typeparam>
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
    /// ȡ������һ�������Ͳ������¼���
    /// ���Ѹ����ܼ���齨���Ż����������˶��ֵ�����ڵļ�飬ʹ�������׳��
    /// </summary>
    public void Unsubscribe<T>(string eventName, Action<T> listener)
    {
        // ����¼��ֵ����Ƿ���ڸ��¼���ί��
        if (eventDictionary.TryGetValue(eventName, out Delegate d))
        {
            Delegate currentDelegate = Delegate.Remove(d, listener);
            if (currentDelegate == null)
            {
                // ����Ƴ���ί��Ϊ�գ�����ֵ����Ƴ����¼���
                eventDictionary.Remove(eventName);
            }
            else
            {
                eventDictionary[eventName] = currentDelegate;
            }
        }
    }

    /// <summary>
    /// ����һ�������Ͳ������¼���
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="eventData">�¼�����</param>
    /// <typeparam name="T">�¼���������</typeparam>
    public void TriggerEvent<T>(string eventName, T eventData)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate d))
        {
            (d as Action<T>)?.Invoke(eventData);
        }
    }

}
