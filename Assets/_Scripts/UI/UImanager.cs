// UImanager.cs (V2.2 - ���չ��ܲ����)
using UnityEngine;
using TMPro;
using System.Collections;

public class UImanager : MonoBehaviour
{
    public static UImanager Instance { get; private set; }

    [Header("UI��������")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject storagePanel;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject questLogPanel;

    [Header("��UI������")]
    [SerializeField] private StorageUIManager storageUIManager;
    [SerializeField] private ShopUI shopUI; // ����ShopPanel�ϵĿ�����

    [Header("ȫ����ʾUI")]
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
    // --- �����ġ����ڴ�/�ر����ĵ��ȷ��� ---

    /// <summary>
    /// ���޸ġ���׼������������������������ͬ�����״̬
    /// </summary>
    public void ToggleSettingsPanel(bool show)
    {
        settingsPanel.SetActive(show);
        PlayerStateMachine.Instance.SetState(show ? PlayerState.InMenu : PlayerState.Gameplay);
    }

    /// <summary>
    /// ���޸ġ���׼��������������־������ (����岻Ӱ�����״̬)
    /// </summary>
    public void ToggleQuestLogPanel(bool show)
    {
        if (questLogPanel != null)
            questLogPanel.SetActive(show);
    }

    /// <summary>
    /// ������������ȱʧ�ķ�������������������������ͬ�����״̬
    /// </summary>
    public void ToggleCraftingPanel(bool show)
    {
        craftingPanel.SetActive(show);
        PlayerStateMachine.Instance.SetState(show ? PlayerState.InMenu : PlayerState.Gameplay);
    }

    // ��ǰ������ӡ�UImanagerֻ���𡰴���ָ�������Ĵ��߼���StorageUIManager�Լ�����
    public void OpenStoragePanel(StorageBoxController storageBox)
    {
        if (storageUIManager != null)
        {
            storageUIManager.Open(storageBox);
            PlayerStateMachine.Instance.SetState(PlayerState.InMenu);
        }
    }

    /// <summary>
    /// ���޸ġ��رղֿ������߼���
    /// �������������йر���صĲ�����
    /// </summary>
    public void CloseStoragePanel()
    {
        // 1. ȷ����屻�ر�
        if (storagePanel != null)
        {
            storagePanel.SetActive(false);
        }

        // 2. �ָ����״̬
        PlayerStateMachine.Instance.SetState(PlayerState.Gameplay);

        // 3. ����һ�����ʵ�ʱ�����ڹرղֿ���Զ�������Ϸ
        DataManager.Instance.SaveGame();

        Debug.Log("[UImanager] �ֿ��ѹرգ����״̬�ѻָ�����Ϸ�ѱ��档");
    }

    // --- ȫ����ʾϵͳ ---
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