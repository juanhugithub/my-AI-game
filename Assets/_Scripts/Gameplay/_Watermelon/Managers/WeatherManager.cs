// WeatherManager.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WeatherSetting
{
    public WeatherType weather;
    public Sprite background;
    public GameObject weatherEffect; // ������Ч���� (�ꡢѩ)
}

public class WeatherManager : MonoBehaviour
{
    public List<WeatherSetting> weatherSettings;
    private Dictionary<WeatherType, WeatherSetting> weatherDictionary;

    // ����UIManager���ı䱳��ͼ
    public Watermelon_UIManager uiManager;

    void Awake()
    {
        // ��ʼ���ֵ�
        weatherDictionary = new Dictionary<WeatherType, WeatherSetting>();
        foreach (var setting in weatherSettings)
        {
            weatherDictionary[setting.weather] = setting;
            // Ĭ�Ϲر�����������Ч
            if (setting.weatherEffect != null)
            {
                setting.weatherEffect.SetActive(false);
            }
        }
    }

    public void SetWeather(WeatherType newWeather)
    {
        // �ر�����������Ч
        foreach (var setting in weatherSettings)
        {
            if (setting.weatherEffect != null)
            {
                setting.weatherEffect.SetActive(false);
            }
        }

        // ����������
        if (weatherDictionary.ContainsKey(newWeather))
        {
            var currentSetting = weatherDictionary[newWeather];

            // 1. ���±���ͼ
            if (uiManager != null && currentSetting.background != null)
            {
                uiManager.SetBackgroundImage(currentSetting.background);
                Debug.Log(currentSetting.background);
            }

            // 2. �����Ӧ��������Ч
            if (currentSetting.weatherEffect != null)
            {
                currentSetting.weatherEffect.SetActive(true);
                Debug.Log("����������Ч�ɹ���");
            }
        }
    }
}
