// SettingsUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // For Dropdown options

public class SettingsUI : MonoBehaviour
{
    [Header("UI控件引用")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;

    private void OnEnable()
    {
        // 每次打开面板时，都从SettingsManager加载当前设置并更新UI
        LoadAndApplySettings();

        // 添加监听器
        masterVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.UpdateMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.UpdateMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.UpdateSfxVolume);
        graphicsDropdown.onValueChanged.AddListener(SettingsManager.Instance.UpdateGraphicsQuality);
    }

    private void OnDisable()
    {
        // 关闭面板时，移除监听器以避免内存泄漏
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        graphicsDropdown.onValueChanged.RemoveAllListeners();
    }

    private void LoadAndApplySettings()
    {
        var settings = SettingsManager.Instance.settingsData;

        // 更新滑块值（不触发事件）
        masterVolumeSlider.SetValueWithoutNotify(settings.masterVolume);
        musicVolumeSlider.SetValueWithoutNotify(settings.musicVolume);
        sfxVolumeSlider.SetValueWithoutNotify(settings.sfxVolume);

        // 动态填充画质选项并设置当前值
        if (graphicsDropdown.options.Count == 0)
        {
            graphicsDropdown.AddOptions(QualitySettings.names.ToList());
        }
        graphicsDropdown.SetValueWithoutNotify(settings.graphicsQualityIndex);
        graphicsDropdown.RefreshShownValue();
    }
}