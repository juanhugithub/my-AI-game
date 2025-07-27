// AudioManager.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ȫ��Ψһ����Ƶ�����������õ���ģʽ��
/// ְ�𣺹���Ͳ������б������֣�Music������Ч��SFX��������Ŀ��������Ƶ���ŵ�Ψһ���ڡ�
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("��Դ���")]
    [SerializeField] private AudioSource musicSource; // ���ڲ���BGM
    [SerializeField] private AudioSource sfxSource;   // ���ڲ���һ���Ե���Ч

    [Header("��Ч��")]
    // ��Inspector�н�������Ϸ��Ч�ϵ�����
    [SerializeField] private AudioClip[] soundClips;
    private Dictionary<string, AudioClip> soundClipDict;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        // ������ת��Ϊ�ֵ䣬����ͨ�����ֵ��ã����Ч��
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
    /// ������Ч�ļ����ƣ�����һ����Ч��
    /// </summary>
    /// <param name="clipName">��Ч�ļ���AudioClip����׼ȷ����</param>
    public void PlaySound(string clipName)
    {
        if (soundClipDict.TryGetValue(clipName, out AudioClip clip))
        {
            // ʹ��PlayOneShot���Դ�������Чͬʱ�����Ҳ������ϵ����
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] �Ҳ�����Ϊ {clipName} ����Ч��");
        }
    }

    // public void PlayMusic(string clipName) { /* δ��ʵ�� */ }
    // public void SetSfxVolume(float volume) { /* δ��ʵ�� */ }
}