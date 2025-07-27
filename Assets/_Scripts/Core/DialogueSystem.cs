// DialogueSystem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System; // For Action

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("UI����")]
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
        // ����Ի�����Ǽ���ģ���������Լ����Ի�
        if (dialoguePanel.activeInHierarchy)
        {
            // ���á�����ˮ�����ĵ�������������Ի�
            if (GameInput.GetDropAction())
            {
                DisplayNextSentence();
            }
        }
    }
    // �����������˶�������ʱ��������Ƿ��ǵ�ǰ�ĵ���ʵ����
    // ����ǣ��򽫾�̬ʵ����Ϊnull����ֹ�����ű����ʵ������ٵĶ���
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
        onDialogueEndCallback?.Invoke(); // ����лص�����ִ��
    }
}