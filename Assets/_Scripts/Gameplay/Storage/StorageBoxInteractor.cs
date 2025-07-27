// StorageBoxInteractor.cs
using UnityEngine;

// 确保该对象上一定有关联的StorageBoxController
[RequireComponent(typeof(StorageBoxController))]
public class StorageBoxInteractor : MonoBehaviour
{
    private StorageBoxController storageBox;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        // 获取挂载在同一个游戏对象上的StorageBoxController组件
        storageBox = GetComponent<StorageBoxController>();
    }

    private void Update()
    {
        // 检查玩家是否在范围内，并且按下了交互键
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // 调用储物箱核心逻辑脚本中的OpenStorage方法
            storageBox.OpenStorage();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入触发器的是否是玩家
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 未来可在此处显示"按E打开"的UI提示
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开触发器的是否是玩家
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 未来可在此处隐藏UI提示
        }
    }
}