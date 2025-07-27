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
    /// ����������ȡ����ƶ���������
    /// </summary>
    public static Vector2 GetMovementAxis()
    {
        // ʹ��Unity����������ᣬ��ӦW,A,S,D�ͷ����
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}