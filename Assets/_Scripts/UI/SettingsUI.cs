// SettingsUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // For Dropdown options

public class SettingsUI : MonoBehaviour
{
    [Header("UI�ؼ�����")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;

    private void OnEnable()
    {
        // ÿ�δ����ʱ������SettingsManager���ص�ǰ���ò�����UI
        LoadAndApplySettings();

        // ��Ӽ�����
        masterVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.UpdateMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.UpdateMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.UpdateSfxVolume);
        graphicsDropdown.onValueChanged.AddListener(SettingsManager.Instance.UpdateGraphicsQuality);
    }

    private void OnDisable()
    {
        // �ر����ʱ���Ƴ��������Ա����ڴ�й©
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        graphicsDropdown.onValueChanged.RemoveAllListeners();
    }

    private void LoadAndApplySettings()
    {
        var settings = SettingsManager.Instance.settingsData;

        // ���»���ֵ���������¼���
        masterVolumeSlider.SetValueWithoutNotify(settings.masterVolume);
        musicVolumeSlider.SetValueWithoutNotify(settings.musicVolume);
        sfxVolumeSlider.SetValueWithoutNotify(settings.sfxVolume);

        // ��̬��仭��ѡ����õ�ǰֵ
        if (graphicsDropdown.options.Count == 0)
        {
            graphicsDropdown.AddOptions(QualitySettings.names.ToList());
        }
        graphicsDropdown.SetValueWithoutNotify(settings.graphicsQualityIndex);
        graphicsDropdown.RefreshShownValue();
    }
}