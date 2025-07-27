// SettingsData.cs
using System;

[Serializable]
public class SettingsData
{
    public float masterVolume = 0.8f;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public int graphicsQualityIndex = 2; // 默认中等画质 (0=Low, 1=Medium, 2=High)
}