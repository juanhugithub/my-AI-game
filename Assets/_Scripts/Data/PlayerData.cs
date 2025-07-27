// PlayerData.cs (�ļ�ͷ������)
using System;
using System.Collections.Generic; // ��Ҫ����

/// <summary>
/// �����л��ı�����Ʒ��λ��
/// ������PlayerData�д洢������Ʒ��Ϣ����Ϊ������ֱ�����л��ֵ䡣
/// ������Ʒ��ID��������
/// </summary>
[Serializable]
public class InventoryItemSlot
{
    public string itemID; // ���ǽ�ʹ����Ʒ�� ScriptableObject �ļ�����ΪΨһID
    public int amount;
}

[Serializable]
public class PlayerData
{
    public long Gold;
    // ������������Ʒ�б�
    public List<InventoryItemSlot> inventoryItems = new List<InventoryItemSlot>();
}