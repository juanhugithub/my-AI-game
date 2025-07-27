// DialogueData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "�ξ�С��/����/�Ի�����")]
public class DialogueData : ScriptableObject
{
    public NPCData speaker; // �Ի���
    [TextArea(3, 10)]
    public string[] sentences; // �Ի���������
}