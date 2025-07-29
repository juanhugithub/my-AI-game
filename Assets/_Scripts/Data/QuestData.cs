// QuestData.cs
using UnityEngine;
using System.Collections.Generic;
using System; // For Serializable

// ObjectiveTypeö���޸�
public enum ObjectiveType { GATHER, PLAY_GAME, TALK, BUY_ITEM, SELL_ITEM } //��������

[Serializable]
public class QuestObjective
{
    public ObjectiveType type;
    public string targetID;       // ��������Ŀ��ID����NPC��GUID�����򴥷��������ֵ�
    public ItemData targetItem; // GATHER������Ҫ
    public int amount;

    [HideInInspector] public int currentAmount; // ����׷�ٽ���
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "�ξ�С��/����/��������")]
public class QuestData : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public List<QuestObjective> objectives;

    [Header("����")]
    public long goldReward;
    public List<InventoryItemSlot> itemRewards;
}