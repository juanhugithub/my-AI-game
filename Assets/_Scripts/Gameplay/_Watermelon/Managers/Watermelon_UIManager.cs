// Watermelon_UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class Watermelon_UIManager : MonoBehaviour
{
    [Header("UI面板与元素")]
    public GameObject startBtn;
    public Text totalScoreText;
    public Text highestScoreText;
    public Image backgroundImage;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("按钮")]
    public Button settingsButton;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;// 这是设置面板里的“回大厅”按钮
    public Button gameOverRestartButton;
    public Button gameOverMenuButton;

    [Header("音量滑块")]// 这些变量可以保留，但我们暂时不处理它们的逻辑
    public Slider bgmSlider;
    public Slider voiceSlider;
    public Slider sfxSlider;

    [Header("管理器引用")]
    public GameManager gameManager;

    [Header("游戏结束UI元素")]
    public Text finalScoreText;

    // --- 道具系统UI ---
    [Header("道具UI")]
    public GameObject luckyFruitPanel; // 幸运果实的选择面板

    [Header("道具详情与数量")]
    public GameObject dissolveInfoPanel;
    public Text dissolveCountText;

    [Header("游戏提示")] // <-- 新增
    public Text tipText;
    public float tipDisplayDuration = 2.0f;
    private Coroutine hideTipCoroutine;

    [Header("道具指示器")] // <-- 新增
    public GameObject targetingIndicator;

    public GameObject blackHoleInfoPanel;
    public Text blackHoleCountText;

    public GameObject luckyFruitInfoPanel; // 这是指幸运果实的图标/详情区域，与选择面板分开
    public Text luckyFruitCountText;

    private ItemType? currentOpenItem = null; // 记录当前打开的道具详情

    void Start()
    { // 【关键修复】: 彻底移除了所有关于 audioManager 和滑块 AddListener 的代码块。
        // 全局音量将在未来的 AudioManager 中统一处理，此UI控制器不应再负责。

        if (startBtn.GetComponent<Button>() != null)
        {
            startBtn.GetComponent<Button>().onClick.AddListener(gameManager.StartGame);
        }
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        resumeButton.onClick.AddListener(CloseSettingsPanel);

        // 【注意】: 我们已经在 Inspector 中将按钮链接到了 WatermelonBridge
        // 为了避免冲突，您在 GameManager.cs 的代码中也不应再为这些按钮添加监听器
        // restartButton.onClick.AddListener(...);  <-- 这些应在Inspector中设置
        // quitButton.onClick.AddListener(...);     <-- 这些应在Inspector中设置

        settingsPanel.SetActive(false);
        if (luckyFruitPanel != null) luckyFruitPanel.SetActive(false);
        CloseAllItemPanels();
        if (tipText != null) tipText.gameObject.SetActive(false);
        if (targetingIndicator != null) targetingIndicator.SetActive(false);
    }
    void Update()
    {
        // 当游戏处于选择目标状态时，让指示器跟随鼠标
        if (targetingIndicator != null && targetingIndicator.activeInHierarchy)
        {
            Vector3 mousePos = Input.mousePosition;
            // 如果Canvas的Render Mode是Screen Space - Camera，需要做转换
            // 这里假设是Screen Space - Overlay
            targetingIndicator.transform.position = mousePos;
        }
    }
    // --- 新增和修改的方法 ---
    // --- 新增方法 ---
    /// <summary>
    /// 显示一条提示信息，并在数秒后自动隐藏
    /// </summary>
    public void ShowTip(string message)
    {
        if (tipText == null) return;

        // 如果之前有隐藏任务在跑，先停掉
        if (hideTipCoroutine != null)
        {
            StopCoroutine(hideTipCoroutine);
        }

        tipText.text = message;
        tipText.gameObject.SetActive(true);
        hideTipCoroutine = StartCoroutine(HideTipAfterDelay());
    }

    private System.Collections.IEnumerator HideTipAfterDelay()
    {
        yield return new WaitForSeconds(tipDisplayDuration);
        tipText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 公共方法，由道具图标按钮点击时调用
    /// </summary>
    public void OnItemIconClicked(int itemTypeAsInt)
    {
        ItemType clickedItem = (ItemType)itemTypeAsInt;

        bool wasAlreadyOpen = currentOpenItem.HasValue && currentOpenItem.Value == clickedItem;

        CloseAllItemPanels(); // 总是先关闭所有

        if (!wasAlreadyOpen) // 如果点击的不是一个已经打开的，则打开它
        {
            switch (clickedItem)
            {
                case ItemType.DissolvePotion:
                    if (dissolveInfoPanel != null) dissolveInfoPanel.SetActive(true);
                    break;
                case ItemType.BlackHoleBomb:
                    if (blackHoleInfoPanel != null) blackHoleInfoPanel.SetActive(true);
                    break;
                case ItemType.LuckyFruit:
                    if (luckyFruitInfoPanel != null) luckyFruitInfoPanel.SetActive(true);
                    break;
            }
            currentOpenItem = clickedItem;
        }
    }

    /// <summary>
    /// 更新指定道具的剩余数量显示
    /// </summary>
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

    /// <summary>
    /// 关闭所有道具的详情面板
    /// </summary>
    public void CloseAllItemPanels()
    {
        if (dissolveInfoPanel != null) dissolveInfoPanel.SetActive(false);
        if (blackHoleInfoPanel != null) blackHoleInfoPanel.SetActive(false);
        if (luckyFruitInfoPanel != null) luckyFruitInfoPanel.SetActive(false);
        currentOpenItem = null;
    }

    // --- 这里是之前缺失的方法 ---
    /// <summary>
    /// 显示或隐藏幸运果实的选择面板
    /// </summary>
    public void ShowLuckyFruitPanel(bool show)
    {
        if (luckyFruitPanel != null)
        {
            luckyFruitPanel.SetActive(show);
        }
    }

    // --- 其他已有方法 ---
    public void OpenSettingsPanel() { settingsPanel.SetActive(true); Time.timeScale = 0f;// 【新增】进入暂停状态
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.gameState = GameState.Paused;
        }
    }
    public void CloseSettingsPanel() { settingsPanel.SetActive(false); Time.timeScale = 1f;// 【新增】恢复到待命状态（或之前的状态，但待命最安全）
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.gameState = GameState.StandBy;
        }
    }
    public void ShowStartMenu() { startBtn.SetActive(true); settingsButton.gameObject.SetActive(false); totalScoreText.gameObject.SetActive(false); }
    public void ShowGameUI() { startBtn.SetActive(false); settingsButton.gameObject.SetActive(true); totalScoreText.gameObject.SetActive(true); }
    public void UpdateCurrentScoreDisplay(float score) { totalScoreText.text = "当前得分：" + score.ToString(); }
    public void UpdateHighestScoreDisplay(float score) { highestScoreText.text = "历史最高：" + score; }
    public void SetBackgroundImage(Sprite sprite) { if (backgroundImage != null) { backgroundImage.sprite = sprite; } }
    public void ShowGameOverPanel(float score)
    {
        if (totalScoreText != null) totalScoreText.gameObject.SetActive(false);
        if (settingsButton != null) settingsButton.gameObject.SetActive(false);
        if (finalScoreText != null) finalScoreText.text = "最终得分: " + score;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
    // --- 新增方法 ---
    /// <summary>
    /// 显示或隐藏目标指示器
    /// </summary>
    public void ShowTargetingIndicator(bool show)
    {
        if (targetingIndicator != null)
        {
            targetingIndicator.SetActive(show);
        }
    }
}