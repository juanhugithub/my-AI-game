// SettingsManager.cs
using UnityEngine;
using UnityEngine.Audio; // ��Ҫ����Audio�����ռ�

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public SettingsData settingsData;

    [Header("��Ƶ����")]
    [SerializeField] private AudioMixer mainMixer;

    private const string SettingsKey = "DreamTownSettings";

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        LoadSettings();
    }

    private void Start()
    {
        // ��Ϸ����ʱӦ��һ���Ѽ��ص�����
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

    // --- ��Ƶ���� ---
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

    // --- ͼ������ ---
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