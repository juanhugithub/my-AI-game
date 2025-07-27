// AudioManager.cs (请确认你的版本与此类似)
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BgmPlaylist
{
    public WeatherType weather;
    public AudioClip[] tracks;
}

public class AudioManager : MonoBehaviour
{
    [Header("音源组件")]
    public AudioSource bgmSource;
    public AudioSource voiceSource;
    public AudioSource sfxSource;

    [Header("BGM播放列表")]
    public List<BgmPlaylist> bgmPlaylists;

    private Dictionary<WeatherType, AudioClip[]> bgmDictionary;
    private WeatherType currentWeather;
    private int currentTrackIndex = -1;

    void Awake()
    {
        bgmDictionary = new Dictionary<WeatherType, AudioClip[]>();
        foreach (var playlist in bgmPlaylists)
        {
            if (!bgmDictionary.ContainsKey(playlist.weather))
            {
                bgmDictionary.Add(playlist.weather, playlist.tracks);
            }
        }
        LoadVolumes();
    }

    void Update()
    {
        if (bgmSource != null && !bgmSource.isPlaying && bgmSource.gameObject.activeInHierarchy)
        {
            PlayNextRandomTrack();
        }
    }

    public void PlayBGM(WeatherType weather)
    {
        currentWeather = weather;
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
        PlayNextRandomTrack();
    }

    private void PlayNextRandomTrack()
    {
        if (bgmDictionary.ContainsKey(currentWeather) && bgmDictionary[currentWeather].Length > 0)
        {
            var tracks = bgmDictionary[currentWeather];
            int nextTrackIndex = Random.Range(0, tracks.Length);
            if (tracks.Length > 1 && nextTrackIndex == currentTrackIndex)
            {
                nextTrackIndex = (nextTrackIndex + 1) % tracks.Length;
            }

            currentTrackIndex = nextTrackIndex;
            bgmSource.clip = tracks[currentTrackIndex];
            bgmSource.Play();
        }
    }

    public void PlayVoice(AudioClip clip)
    {
       if (voiceSource != null && clip != null)
    {
        // 【方案A】如果当前没在播放，才播放新的 (简单粗暴)
        if (!voiceSource.isPlaying)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
        }

        
    }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetVoiceVolume(float volume)
    {
        if (voiceSource != null) voiceSource.volume = volume;
        PlayerPrefs.SetFloat("VoiceVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolumes()
    {
        // --- 关键修改在这里 ---
        // PlayerPrefs.GetFloat("键名", 默认值);
        // 默认值是一个0到1之间的浮点数 (float)

        // BGM 默认音量 60%
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.6f);
        // 语音 默认音量 70%
        float voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 0.7f);
        // 音效 默认音量 50%
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        // 应用这些值
        if (bgmSource != null) bgmSource.volume = bgmVolume;
        if (voiceSource != null) voiceSource.volume = voiceVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }
}
