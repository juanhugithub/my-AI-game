using UnityEngine;
public class PlayerStateMachine : MonoBehaviour
{
    public static PlayerStateMachine Instance { get; private set; }
    public PlayerState CurrentState { get; private set; }
    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        CurrentState = PlayerState.Gameplay;
    }
    public void SetState(PlayerState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
    }
}