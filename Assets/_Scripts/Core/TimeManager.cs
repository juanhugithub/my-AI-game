// TimeManager.cs
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI clockText;
    [Tooltip("真实世界多少秒等于游戏内1小时")]
    [SerializeField] private float secondsPerHour = 5f;

    public int CurrentDay { get; private set; } = 1;
    public int CurrentHour { get; private set; } = 6; // 早上6点开始

    private float timer;

    private void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerHour)
        {
            timer = 0;
            CurrentHour++;
            if (CurrentHour >= 24)
            {
                CurrentHour = 0;
                CurrentDay++;
                // 广播日终事件
                EventManager.Instance.TriggerEvent<int>(GameEvents.OnDayEnd, CurrentDay);
            }
            UpdateClockUI();
        }
    }

    private void UpdateClockUI()
    {
        if (clockText != null)
        {
            clockText.text = $"第 {CurrentDay} 天 {CurrentHour:00}:00";
        }
    }
}