// PlayerPersistence.cs
using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 使该玩家对象在加载场景时不被销毁
        }
        else
        {
            // 如果场景中已存在一个持久化玩家，则销毁后来者
            Destroy(gameObject);
        }
    }
}