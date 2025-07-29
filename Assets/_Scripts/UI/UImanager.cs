// UImanager.cs (V2.2 - 最终功能补完版)
using UnityEngine;
using TMPro;
using System.Collections;

public class UImanager : MonoBehaviour
{
    public static UImanager Instance { get; private set; }

    [Header("UI面板根对象")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject storagePanel;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject questLogPanel;

    [Header("子UI管理器")]
    [SerializeField] private StorageUIManager storageUIManager;
    [SerializeField] private ShopUI shopUI; // 引用ShopPanel上的控制器

    [Header("全局提示UI")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private float hintDisplayDuration = 3f;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
    }
    public void OpenShopPanel(ShopData shopData)
    {
        if (shopUI != null)
        {
            shopUI.OpenShop(shopData);
            PlayerStateMachine.Instance.SetState(PlayerState.InMenu);
        }
    }

    public void CloseShopPanel()
    {
        PlayerStateMachine.Instance.SetState(PlayerState.Gameplay);
    }
    // --- 公共的、用于打开/关闭面板的调度方法 ---

    /// <summary>
    /// 【修改】标准化：控制设置面板的显隐，并同步玩家状态
    /// </summary>
    public void ToggleSettingsPanel(bool show)
    {
        settingsPanel.SetActive(show);
        PlayerStateMachine.Instance.SetState(show ? PlayerState.InMenu : PlayerState.Gameplay);
    }

    /// <summary>
    /// 【修改】标准化：控制任务日志的显隐 (此面板不影响玩家状态)
    /// </summary>
    public void ToggleQuestLogPanel(bool show)
    {
        if (questLogPanel != null)
            questLogPanel.SetActive(show);
    }

    /// <summary>
    /// 【新增】补完缺失的方法：控制制造面板的显隐，并同步玩家状态
    /// </summary>
    public void ToggleCraftingPanel(bool show)
    {
        craftingPanel.SetActive(show);
        PlayerStateMachine.Instance.SetState(show ? PlayerState.InMenu : PlayerState.Gameplay);
    }

    // 【前后端连接】UImanager只负责“传达指令”，具体的打开逻辑由StorageUIManager自己处理
    public void OpenStoragePanel(StorageBoxController storageBox)
    {
        if (storageUIManager != null)
        {
            storageUIManager.Open(storageBox);
            PlayerStateMachine.Instance.SetState(PlayerState.InMenu);
        }
    }

    /// <summary>
    /// 【修改】关闭仓库面板的逻辑。
    /// 现在它负责所有关闭相关的操作。
    /// </summary>
    public void CloseStoragePanel()
    {
        // 1. 确保面板被关闭
        if (storagePanel != null)
        {
            storagePanel.SetActive(false);
        }

        // 2. 恢复玩家状态
        PlayerStateMachine.Instance.SetState(PlayerState.Gameplay);

        // 3. 这是一个合适的时机，在关闭仓库后自动保存游戏
        DataManager.Instance.SaveGame();

        Debug.Log("[UImanager] 仓库已关闭，玩家状态已恢复，游戏已保存。");
    }

    // --- 全局提示系统 ---
    public void ShowGlobalHint(string message)
    {
        StartCoroutine(ShowHintRoutine(message));
    }

    private IEnumerator ShowHintRoutine(string message)
    {
        if (hintPanel == null || hintText == null) yield break;
        hintText.text = message;
        hintPanel.SetActive(true);
        yield return new WaitForSeconds(hintDisplayDuration);
        hintPanel.SetActive(false);
    }
}