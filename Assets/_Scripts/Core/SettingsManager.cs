// SettingsManager.cs
using UnityEngine;
using UnityEngine.Audio; // 需要引入Audio命名空间

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public SettingsData settingsData;

    [Header("音频设置")]
    [SerializeField] private AudioMixer mainMixer;

    private const string SettingsKey = "DreamTownSettings";

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        LoadSettings();
    }

    private void Start()
    {
        // 游戏启动时应用一次已加载的设置
        ApplyAllSettings();
    }

    public void LoadSettings()
    {
        string json = PlayerPrefs.GetString(SettingsKey, string.Empty);
        settingsData = !string.IsNullOrEmpty(json)
            ? JsonUtility.FromJson<SettingsData>(json)
            : new SettingsData();
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settingsData);
        PlayerPrefs.SetString(SettingsKey, json);
        PlayerPrefs.Save();
    }

    public void ApplyAllSettings()
    {
        ApplyAudioSettings();
        ApplyGraphicsSettings();
    }

    // --- 音频设置 ---
    public void ApplyAudioSettings()
    {
        if (mainMixer == null) return;
        mainMixer.SetFloat("MasterVolume", ConvertToDecibel(settingsData.masterVolume));
        mainMixer.SetFloat("MusicVolume", ConvertToDecibel(settingsData.musicVolume));
        mainMixer.SetFloat("SFXVolume", ConvertToDecibel(settingsData.sfxVolume));
    }

    private float ConvertToDecibel(float volume)
    {
        return volume > 0.001f ? Mathf.Log10(volume) * 20 : -80f;
    }

    public void UpdateMasterVolume(float volume) { settingsData.masterVolume = volume; ApplyAudioSettings(); SaveSettings(); }
    public void UpdateMusicVolume(float volume) { settingsData.musicVolume = volume; ApplyAudioSettings(); SaveSettings(); }
    public void UpdateSfxVolume(float volume) { settingsData.sfxVolume = volume; ApplyAudioSettings(); SaveSettings(); }

    // --- 图形设置 ---
    public void ApplyGraphicsSettings()
    {
        int qualityIndex = Mathf.Clamp(settingsData.graphicsQualityIndex, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    public void UpdateGraphicsQuality(int index)
    {
        settingsData.graphicsQualityIndex = index;
        ApplyGraphicsSettings();
        SaveSettings();
    }
}