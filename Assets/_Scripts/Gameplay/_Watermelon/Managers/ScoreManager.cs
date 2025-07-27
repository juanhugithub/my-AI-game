// ScoreManager.cs
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float TotalScore { get; private set; } = 0f;
    public float HighestScore { get; private set; } = 0f;

    private const string HighestScoreKey = "HighestScore";

    // 引用UI管理器
    public UIManager uiManager;

    public void AddScore(float scoreToAdd)
    {
        TotalScore += scoreToAdd;
        uiManager.UpdateCurrentScoreDisplay(TotalScore);
    }

    public void ResetCurrentScore()
    {
        TotalScore = 0f;
        uiManager.UpdateCurrentScoreDisplay(TotalScore);
    }

    public void LoadAndDisplayHighestScore()
    {
        HighestScore = PlayerPrefs.GetFloat(HighestScoreKey, 0);
        uiManager.UpdateHighestScoreDisplay(HighestScore);
    }

    public void SaveHighestScore()
    {
        if (TotalScore > HighestScore)
        {
            PlayerPrefs.SetFloat(HighestScoreKey, TotalScore);
        }
    }
}
