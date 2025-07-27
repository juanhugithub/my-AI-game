// GameInput.cs
using UnityEngine;

public static class GameInput
{
    public static bool GetDropAction()
    {
        return Input.GetMouseButtonDown(0);
    }

    public static bool GetInteractActionDown()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    /// <summary>
    /// 【新增】获取玩家移动输入轴向。
    /// </summary>
    public static Vector2 GetMovementAxis()
    {
        // 使用Unity经典的输入轴，对应W,A,S,D和方向键
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}