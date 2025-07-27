// FruitSpawner.cs
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("水果预制体数组")]
    public GameObject[] fruitPrefabs;
    [Header("生成位置")]
    public Transform spawnPosition;

    [Header("移动范围限制")]
    public float minX = -2.5f; // 左边界
    public float maxX = 2.5f;  // 右边界

    private GameObject pendingFruit;

    /// <summary>
    /// 创建一个随机的水果
    /// </summary>
    public void CreateFruit()
    {
        // 如果已经有一个待命水果，先销毁它，防止重复创建
        if (pendingFruit != null)
        {
            // 如果已经有一个待命水果了，就直接返回，不再创建新的。
            return;
        }

        int index = Random.Range(0, 5); // 默认只生成前5种基础水果
        GameObject newFruit = Instantiate(fruitPrefabs[index], spawnPosition);
        newFruit.transform.localPosition = Vector3.zero;
        newFruit.GetComponent<Rigidbody2D>().isKinematic = true;
        pendingFruit = newFruit;
    }

    // --- 新增方法 ---
    /// <summary>
    /// （为“幸运果实”道具新增）创建一个指定类型的水果作为待命水果
    /// </summary>
    /// <param name="index">水果在 prefabs 数组中的索引</param>
    public void CreateSpecificFruit(int index)
    {
        // 同样，先销毁可能存在的待命水果
        if (pendingFruit != null)
        {
            Destroy(pendingFruit); // 如果已经有了，先销毁旧的，再创建新的
        }

        if (index < 0 || index >= fruitPrefabs.Length)
        {
            Debug.LogError("无效的水果索引！将创建一个随机水果作为替代。");
            CreateFruit(); // 创建一个随机的作为备用
            return;
        }

        GameObject newFruit = Instantiate(fruitPrefabs[index], spawnPosition);
        newFruit.transform.localPosition = Vector3.zero;
        newFruit.GetComponent<Rigidbody2D>().isKinematic = true;
        pendingFruit = newFruit;
    }


    /// <summary>
    /// 掉落当前待命的水果
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
    /// 控制待命水果左右移动
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
    /// 合并后生成一个更高级的水果
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