// WeatherManager.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WeatherSetting
{
    public WeatherType weather;
    public Sprite background;
    public GameObject weatherEffect; // 天气特效对象 (雨、雪)
}

public class WeatherManager : MonoBehaviour
{
    public List<WeatherSetting> weatherSettings;
    private Dictionary<WeatherType, WeatherSetting> weatherDictionary;

    // 引用UIManager来改变背景图
    public Watermelon_UIManager uiManager;

    void Awake()
    {
        // 初始化字典
        weatherDictionary = new Dictionary<WeatherType, WeatherSetting>();
        foreach (var setting in weatherSettings)
        {
            weatherDictionary[setting.weather] = setting;
            // 默认关闭所有天气特效
            if (setting.weatherEffect != null)
            {
                setting.weatherEffect.SetActive(false);
            }
        }
    }

    public void SetWeather(WeatherType newWeather)
    {
        // 关闭所有天气特效
        foreach (var setting in weatherSettings)
        {
            if (setting.weatherEffect != null)
            {
                setting.weatherEffect.SetActive(false);
            }
        }

        // 设置新天气
        if (weatherDictionary.ContainsKey(newWeather))
        {
            var currentSetting = weatherDictionary[newWeather];

            // 1. 更新背景图
            if (uiManager != null && currentSetting.background != null)
            {
                uiManager.SetBackgroundImage(currentSetting.background);
                Debug.Log(currentSetting.background);
            }

            // 2. 激活对应的天气特效
            if (currentSetting.weatherEffect != null)
            {
                currentSetting.weatherEffect.SetActive(true);
                Debug.Log("激活天气特效成功！");
            }
        }
    }
}
