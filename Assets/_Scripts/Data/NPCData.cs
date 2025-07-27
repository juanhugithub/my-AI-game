// NPCData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "梦境小镇/数据/NPC数据")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public Sprite portrait; // NPC头像
}