// AudioManager.cs (������BGM����)
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("��Դ���")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("��Ч��")]
    [SerializeField] private AudioClip[] soundClips;
    private Dictionary<string, AudioClip> soundClipDict;

    [Header("BGM�����б�")] // ��������
    [SerializeField] private AudioClip[] bgmPlaylist;
    private int currentBgmIndex = -1;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        soundClipDict = new Dictionary<string, AudioClip>();
        foreach (var clip in soundClips)
        {
            if (clip != null && !soundClipDict.ContainsKey(clip.name))
            {
                soundClipDict[clip.name] = clip;
            }
        }
    }

    private void Start()
    {
        // ����������Ϸһ��ʼ�Ͳ��ű�������
        PlayNextBgmTrack();
    }

    private void Update()
    {
        // ����������BGM������Ϻ��Զ�������һ�ף�ʵ��ѭ��
        if (musicSource != null && !musicSource.isPlaying && bgmPlaylist.Length > 0)
        {
            PlayNextBgmTrack();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void PlaySound(string clipName)
    {
        if (soundClipDict.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] �Ҳ�����Ϊ {clipName} ����Ч��");
        }
    }

    /// <summary>
    /// ������������BGM�б��е���һ�׸���
    /// </summary>
    private void PlayNextBgmTrack()
    {
        if (bgmPlaylist == null || bgmPlaylist.Length == 0) return;

        // �򵥵�˳�򲥷ţ����׺��ͷ��ʼ
        currentBgmIndex = (currentBgmIndex + 1) % bgmPlaylist.Length;
        musicSource.clip = bgmPlaylist[currentBgmIndex];
        musicSource.Play();
    }
}