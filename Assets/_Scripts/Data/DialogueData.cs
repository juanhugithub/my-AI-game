// DialogueData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "梦境小镇/数据/对话数据")]
public class DialogueData : ScriptableObject
{
    public NPCData speaker; // 对话者
    [TextArea(3, 10)]
    public string[] sentences; // 对话句子数组
}