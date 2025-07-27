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
    // ����������״̬
    public List<string> activeQuests = new List<string>();
    // ����������������б�
    public List<string> completedQuests = new List<string>();
    // �������洢���д����������
    public List<StorageData> storageBoxData = new List<StorageData>();
}


// ���������ڴ洢�������������ݵĿ����л���
[Serializable]
public class StorageData
{
    public string boxID; // ����Ψһ��ʶ������
    public List<InventoryItemSlot> items = new List<InventoryItemSlot>();
}
