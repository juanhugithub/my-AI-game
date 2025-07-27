// DialogueSystem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System; // For Action

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("UI引用")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerNameText;

    private Queue<string> sentences;
    private Action onDialogueEndCallback;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        sentences = new Queue<string>();
    }

    private void Update()
    {
        // 如果对话面板是激活的，监听点击以继续对话
        if (dialoguePanel.activeInHierarchy)
        {
            // 复用“掉落水果”的点击操作来继续对话
            if (GameInput.GetDropAction())
            {
                DisplayNextSentence();
            }
        }
    }
    // 【新增】当此对象被销毁时，检查它是否是当前的单例实例。
    // 如果是，则将静态实例设为null，防止其他脚本访问到已销毁的对象。
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    public void StartDialogue(DialogueData dialogue, Action onDialogueEnd = null)
    {
        dialoguePanel.SetActive(true);
        sentences.Clear();

        portraitImage.sprite = dialogue.speaker.portrait;
        speakerNameText.text = dialogue.speaker.npcName;
        this.onDialogueEndCallback = onDialogueEnd;

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        onDialogueEndCallback?.Invoke(); // 如果有回调，则执行
    }
}