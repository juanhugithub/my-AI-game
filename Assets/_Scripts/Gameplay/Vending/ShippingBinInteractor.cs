// ShippingBinInteractor.cs
using UnityEngine;

/// <summary>
/// 出货箱的专用交互器。
/// 职责单一：只负责检测玩家的靠近和交互输入，
/// 然后调用挂载在同一个对象上的ShippingBinController的核心方法。
/// </summary>
[RequireComponent(typeof(ShippingBinController))] // 强制要求该对象上必须有ShippingBinController
public class ShippingBinInteractor : MonoBehaviour
{
    // 对出货箱核心逻辑脚本的引用
    private ShippingBinController shippingBin;
    // 标记玩家是否在交互范围内
    private bool isPlayerInRange = false;

    private void Awake()
    {
        // 在唤醒时，自动获取挂载在同一个游戏对象上的ShippingBinController组件
        shippingBin = GetComponent<ShippingBinController>();
    }

    private void Update()
    {
        // 每一帧都检查：玩家是否在范围内，并且是否按下了交互键
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // 如果条件满足，则调用出货箱核心逻辑脚本中的Interact方法
            shippingBin.Interact();
        }
    }

    // 当其他碰撞体进入此对象的触发器时，由Unity自动调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的是否是带有"Player"标签的对象
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 未来可在此处显示"按E放入物品"的UI提示
        }
    }

    // 当其他碰撞体离开此对象的触发器时，由Unity自动调用
    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开的是否是玩家
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 未来可在此处隐藏UI提示
        }
    }
}