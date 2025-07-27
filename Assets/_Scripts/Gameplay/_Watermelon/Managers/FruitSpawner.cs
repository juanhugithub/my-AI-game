// FruitSpawner.cs
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("ˮ��Ԥ��������")]
    public GameObject[] fruitPrefabs;
    [Header("����λ��")]
    public Transform spawnPosition;

    [Header("�ƶ���Χ����")]
    public float minX = -2.5f; // ��߽�
    public float maxX = 2.5f;  // �ұ߽�

    private GameObject pendingFruit;

    /// <summary>
    /// ����һ�������ˮ��
    /// </summary>
    public void CreateFruit()
    {
        // ����Ѿ���һ������ˮ����������������ֹ�ظ�����
        if (pendingFruit != null)
        {
            // ����Ѿ���һ������ˮ���ˣ���ֱ�ӷ��أ����ٴ����µġ�
            return;
        }

        int index = Random.Range(0, 5); // Ĭ��ֻ����ǰ5�ֻ���ˮ��
        GameObject newFruit = Instantiate(fruitPrefabs[index], spawnPosition);
        newFruit.transform.localPosition = Vector3.zero;
        newFruit.GetComponent<Rigidbody2D>().isKinematic = true;
        pendingFruit = newFruit;
    }

    // --- �������� ---
    /// <summary>
    /// ��Ϊ�����˹�ʵ����������������һ��ָ�����͵�ˮ����Ϊ����ˮ��
    /// </summary>
    /// <param name="index">ˮ���� prefabs �����е�����</param>
    public void CreateSpecificFruit(int index)
    {
        // ͬ���������ٿ��ܴ��ڵĴ���ˮ��
        if (pendingFruit != null)
        {
            Destroy(pendingFruit); // ����Ѿ����ˣ������پɵģ��ٴ����µ�
        }

        if (index < 0 || index >= fruitPrefabs.Length)
        {
            Debug.LogError("��Ч��ˮ��������������һ�����ˮ����Ϊ�����");
            CreateFruit(); // ����һ���������Ϊ����
            return;
        }

        GameObject newFruit = Instantiate(fruitPrefabs[index], spawnPosition);
        newFruit.transform.localPosition = Vector3.zero;
        newFruit.GetComponent<Rigidbody2D>().isKinematic = true;
        pendingFruit = newFruit;
    }


    /// <summary>
    /// ���䵱ǰ������ˮ��
    /// </summary>
    public void DropFruit()
    {
        if (pendingFruit != null)
        {
            pendingFruit.transform.parent = null;
            pendingFruit.GetComponent<Rigidbody2D>().isKinematic = false;
            pendingFruit = null;
        }
    }

    /// <summary>
    /// ���ƴ���ˮ�������ƶ�
    /// </summary>
    public void MovePendingFruit(Vector3 targetPosition)
    {
        if (pendingFruit != null)
        {
            float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
            pendingFruit.transform.position = new Vector3(clampedX, spawnPosition.position.y, 0);
        }
    }

    /// <summary>
    /// �ϲ�������һ�����߼���ˮ��
    /// </summary>
    public void Combine(FruitType type, Vector3 position)
    {
        if ((int)type < fruitPrefabs.Length - 1)
        {
            int newIndex = (int)type + 1;
            Instantiate(fruitPrefabs[newIndex], position, Quaternion.identity);
        }
    }
}