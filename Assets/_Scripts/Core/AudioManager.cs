// AudioManager.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 全局唯一的音频管理器，采用单例模式。
/// 职责：管理和播放所有背景音乐（Music）和音效（SFX），是项目中所有音频播放的唯一出口。
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音源组件")]
    [SerializeField] private AudioSource musicSource; // 用于播放BGM
    [SerializeField] private AudioSource sfxSource;   // 用于播放一次性的音效

    [Header("音效库")]
    // 在Inspector中将所有游戏音效拖到这里
    [SerializeField] private AudioClip[] soundClips;
    private Dictionary<string, AudioClip> soundClipDict;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        // 将数组转换为字典，方便通过名字调用，提高效率
        soundClipDict = new Dictionary<string, AudioClip>();
        foreach (var clip in soundClips)
        {
            if (!soundClipDict.ContainsKey(clip.name))
            {
                soundClipDict[clip.name] = clip;
            }
        }
    }

    /// <summary>
    /// 根据音效文件名称，播放一个音效。
    /// </summary>
    /// <param name="clipName">音效文件（AudioClip）的准确名称</param>
    public void PlaySound(string clipName)
    {
        if (soundClipDict.TryGetValue(clipName, out AudioClip clip))
        {
            // 使用PlayOneShot可以处理多个音效同时播放且不互相打断的情况
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] 找不到名为 {clipName} 的音效。");
        }
    }

    // public void PlayMusic(string clipName) { /* 未来实现 */ }
    // public void SetSfxVolume(float volume) { /* 未来实现 */ }
}