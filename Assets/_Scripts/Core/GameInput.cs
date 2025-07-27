// GameInput.cs
using UnityEngine;

public static class GameInput
{
    public static bool GetDropAction()
    {
        return Input.GetMouseButtonDown(0);
    }
}