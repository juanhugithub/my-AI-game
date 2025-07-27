// Watermelon_UIManager.cs (V2.1 - 完整功能最终版)
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // For Coroutine
using TMPro; // Assuming you are using TextMeshPro

public class Watermelon_UIManager : MonoBehaviour
{
    [Header("管理器引用")]
    public GameManager gameManager;

    [Header("UI面板与元素")]
    public GameObject startBtn;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI highestScoreText;
    public Image backgroundImage;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("游戏结束UI元素")]
    public TextMeshProUGUI finalScoreText;

    [Header("道具UI")]
    public GameObject luckyFruitPanel;
    public GameObject dissolveInfoPanel;
    public Text dissolveCountText;
    public GameObject blackHoleInfoPanel;
    public Text blackHoleCountText;
    public GameObject luckyFruitInfoPanel;
    public Text luckyFruitCountText;

    [Header("游戏提示")]
    public TextMeshProUGUI tipText;
    public float tipDisplayDuration = 2.0f;
    private Coroutine hideTipCoroutine;

    [Header("道具指示器")]
    public GameObject targetingIndicator;

    private ItemType? currentOpenItem = null;

    void Awake()
    {
        // 初始化UI状态
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (luckyFruitPanel != null) luckyFruitPanel.SetActive(false);
        CloseAllItemPanels();
        if (tipText != null) tipText.gameObject.SetActive(false);
        if (targetingIndicator != null) targetingIndicator.SetActive(false);
    }

    void Update()
    {
        if (targetingIndicator != null && targetingIndicator.activeInHierarchy)
        {
            Vector3 mousePos = Input.mousePosition;
            targetingIndicator.transform.position = mousePos;
        }
    }

    // --- 【关键】以下是恢复的所有必需的公共方法 ---

    public void UpdateCurrentScoreDisplay(float score)
    {
        if (totalScoreText != null) totalScoreText.text = "当前得分：" + Mathf.FloorToInt(score);
    }

    public void UpdateHighestScoreDisplay(float score)
    {
        if (highestScoreText != null) highestScoreText.text = "历史最高：" + Mathf.FloorToInt(score);
    }

    public void SetBackgroundImage(Sprite sprite)
    {
        if (backgroundImage != null) backgroundImage.sprite = sprite;
    }

    public void ShowStartMenu()
    {
        if (startBtn != null) startBtn.SetActive(true);
        if (totalScoreText != null) totalScoreText.gameObject.SetActive(false);
    }

    public void ShowGameUI()
    {
        if (startBtn != null) startBtn.SetActive(false);
        if (totalScoreText != null) totalScoreText.gameObject.SetActive(true);
    }

    public void UpdateItemCountDisplay(ItemType type, int count)
    {
        switch (type)
        {
            case ItemType.DissolvePotion:
                if (dissolveCountText != null) dissolveCountText.text = count.ToString();
                break;
            case ItemType.BlackHoleBomb:
                if (blackHoleCountText != null) blackHoleCountText.text = count.ToString();
                break;
            case ItemType.LuckyFruit:
                if (luckyFruitCountText != null) luckyFruitCountText.text = count.ToString();
                break;
        }
    }

    public void ShowTip(string message)
    {
        if (tipText == null) return;
        if (hideTipCoroutine != null) StopCoroutine(hideTipCoroutine);
        tipText.text = message;
        tipText.gameObject.SetActive(true);
        hideTipCoroutine = StartCoroutine(HideTipAfterDelay());
    }

    private IEnumerator HideTipAfterDelay()
    {
        yield return new WaitForSeconds(tipDisplayDuration);
        if (tipText != null) tipText.gameObject.SetActive(false);
    }

    public void CloseAllItemPanels()
    {
        if (dissolveInfoPanel != null) dissolveInfoPanel.SetActive(false);
        if (blackHoleInfoPanel != null) blackHoleInfoPanel.SetActive(false);
        if (luckyFruitInfoPanel != null) luckyFruitInfoPanel.SetActive(false);
        currentOpenItem = null;
    }

    public void ShowTargetingIndicator(bool show)
    {
        if (targetingIndicator != null) targetingIndicator.SetActive(show);
    }

    public void ShowLuckyFruitPanel(bool show)
    {
        if (luckyFruitPanel != null) luckyFruitPanel.SetActive(show);
    }

    public void ShowGameOverPanel(float score)
    {
        if (totalScoreText != null) totalScoreText.gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "最终得分: " + Mathf.FloorToInt(score);
    }

    // --- 设置面板的内部控制逻辑 ---
    public void OpenSettingsPanel()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        Time.timeScale = 0f;
        if (gameManager != null) gameManager.gameState = GameState.Paused;
    }

    public void CloseSettingsPanel()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        if (gameManager != null) gameManager.gameState = GameState.StandBy;
    }
}