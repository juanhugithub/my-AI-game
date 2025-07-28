// QuestData.cs
using UnityEngine;
using System.Collections.Generic;
using System; // For Serializable

public enum ObjectiveType { GATHER, PLAY_GAME, TALK } //【新增】对话任务类型

[Serializable]
public class QuestObjective
{
    public ObjectiveType type;
    public string targetID;       // 【新增】目标ID。如NPC的GUID，区域触发器的名字等
    public ItemData targetItem; // GATHER类型需要
    public int amount;

    [HideInInspector] public int currentAmount; // 用于追踪进度
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "梦境小镇/数据/任务数据")]
public class QuestData : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public List<QuestObjective> objectives;

    [Header("奖励")]
    public long goldReward;
    public List<InventoryItemSlot> itemRewards;
}