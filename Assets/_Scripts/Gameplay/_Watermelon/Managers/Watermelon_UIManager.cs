// Watermelon_UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class Watermelon_UIManager : MonoBehaviour
{
    [Header("UI�����Ԫ��")]
    public GameObject startBtn;
    public Text totalScoreText;
    public Text highestScoreText;
    public Image backgroundImage;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("��ť")]
    public Button settingsButton;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;// �������������ġ��ش�������ť
    public Button gameOverRestartButton;
    public Button gameOverMenuButton;

    [Header("��������")]// ��Щ�������Ա�������������ʱ���������ǵ��߼�
    public Slider bgmSlider;
    public Slider voiceSlider;
    public Slider sfxSlider;

    [Header("����������")]
    public GameManager gameManager;

    [Header("��Ϸ����UIԪ��")]
    public Text finalScoreText;

    // --- ����ϵͳUI ---
    [Header("����UI")]
    public GameObject luckyFruitPanel; // ���˹�ʵ��ѡ�����

    [Header("��������������")]
    public GameObject dissolveInfoPanel;
    public Text dissolveCountText;

    [Header("��Ϸ��ʾ")] // <-- ����
    public Text tipText;
    public float tipDisplayDuration = 2.0f;
    private Coroutine hideTipCoroutine;

    [Header("����ָʾ��")] // <-- ����
    public GameObject targetingIndicator;

    public GameObject blackHoleInfoPanel;
    public Text blackHoleCountText;

    public GameObject luckyFruitInfoPanel; // ����ָ���˹�ʵ��ͼ��/����������ѡ�����ֿ�
    public Text luckyFruitCountText;

    private ItemType? currentOpenItem = null; // ��¼��ǰ�򿪵ĵ�������

    void Start()
    { // ���ؼ��޸���: �����Ƴ������й��� audioManager �ͻ��� AddListener �Ĵ���顣
        // ȫ����������δ���� AudioManager ��ͳһ������UI��������Ӧ�ٸ���

        if (startBtn.GetComponent<Button>() != null)
        {
            startBtn.GetComponent<Button>().onClick.AddListener(gameManager.StartGame);
        }
        settingsButton.onClick.AddListener(OpenSettingsPanel);
        resumeButton.onClick.AddListener(CloseSettingsPanel);

        // ��ע�⡿: �����Ѿ��� Inspector �н���ť���ӵ��� WatermelonBridge
        // Ϊ�˱����ͻ������ GameManager.cs �Ĵ�����Ҳ��Ӧ��Ϊ��Щ��ť��Ӽ�����
        // restartButton.onClick.AddListener(...);  <-- ��ЩӦ��Inspector������
        // quitButton.onClick.AddListener(...);     <-- ��ЩӦ��Inspector������

        settingsPanel.SetActive(false);
        if (luckyFruitPanel != null) luckyFruitPanel.SetActive(false);
        CloseAllItemPanels();
        if (tipText != null) tipText.gameObject.SetActive(false);
        if (targetingIndicator != null) targetingIndicator.SetActive(false);
    }
    void Update()
    {
        // ����Ϸ����ѡ��Ŀ��״̬ʱ����ָʾ���������
        if (targetingIndicator != null && targetingIndicator.activeInHierarchy)
        {
            Vector3 mousePos = Input.mousePosition;
            // ���Canvas��Render Mode��Screen Space - Camera����Ҫ��ת��
            // ���������Screen Space - Overlay
            targetingIndicator.transform.position = mousePos;
        }
    }
    // --- �������޸ĵķ��� ---
    // --- �������� ---
    /// <summary>
    /// ��ʾһ����ʾ��Ϣ������������Զ�����
    /// </summary>
    public void ShowTip(string message)
    {
        if (tipText == null) return;

        // ���֮ǰ�������������ܣ���ͣ��
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
    /// �����������ɵ���ͼ�갴ť���ʱ����
    /// </summary>
    public void OnItemIconClicked(int itemTypeAsInt)
    {
        ItemType clickedItem = (ItemType)itemTypeAsInt;

        bool wasAlreadyOpen = currentOpenItem.HasValue && currentOpenItem.Value == clickedItem;

        CloseAllItemPanels(); // �����ȹر�����

        if (!wasAlreadyOpen) // �������Ĳ���һ���Ѿ��򿪵ģ������
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
    /// ����ָ�����ߵ�ʣ��������ʾ
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
    /// �ر����е��ߵ��������
    /// </summary>
    public void CloseAllItemPanels()
    {
        if (dissolveInfoPanel != null) dissolveInfoPanel.SetActive(false);
        if (blackHoleInfoPanel != null) blackHoleInfoPanel.SetActive(false);
        if (luckyFruitInfoPanel != null) luckyFruitInfoPanel.SetActive(false);
        currentOpenItem = null;
    }

    // --- ������֮ǰȱʧ�ķ��� ---
    /// <summary>
    /// ��ʾ���������˹�ʵ��ѡ�����
    /// </summary>
    public void ShowLuckyFruitPanel(bool show)
    {
        if (luckyFruitPanel != null)
        {
            luckyFruitPanel.SetActive(show);
        }
    }

    // --- �������з��� ---
    public void OpenSettingsPanel() { settingsPanel.SetActive(true); Time.timeScale = 0f;// ��������������ͣ״̬
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.gameState = GameState.Paused;
        }
    }
    public void CloseSettingsPanel() { settingsPanel.SetActive(false); Time.timeScale = 1f;// ���������ָ�������״̬����֮ǰ��״̬���������ȫ��
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.gameState = GameState.StandBy;
        }
    }
    public void ShowStartMenu() { startBtn.SetActive(true); settingsButton.gameObject.SetActive(false); totalScoreText.gameObject.SetActive(false); }
    public void ShowGameUI() { startBtn.SetActive(false); settingsButton.gameObject.SetActive(true); totalScoreText.gameObject.SetActive(true); }
    public void UpdateCurrentScoreDisplay(float score) { totalScoreText.text = "��ǰ�÷֣�" + score.ToString(); }
    public void UpdateHighestScoreDisplay(float score) { highestScoreText.text = "��ʷ��ߣ�" + score; }
    public void SetBackgroundImage(Sprite sprite) { if (backgroundImage != null) { backgroundImage.sprite = sprite; } }
    public void ShowGameOverPanel(float score)
    {
        if (totalScoreText != null) totalScoreText.gameObject.SetActive(false);
        if (settingsButton != null) settingsButton.gameObject.SetActive(false);
        if (finalScoreText != null) finalScoreText.text = "���յ÷�: " + score;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
    // --- �������� ---
    /// <summary>
    /// ��ʾ������Ŀ��ָʾ��
    /// </summary>
    public void ShowTargetingIndicator(bool show)
    {
        if (targetingIndicator != null)
        {
            targetingIndicator.SetActive(show);
        }
    }
}