// FarmUIManager.cs (�ռ���׳�� V2 - ʹ��ȫ�ֲ���)
using UnityEngine;

public class FarmUIManager : MonoBehaviour
{
    public static FarmUIManager Instance { get; private set; }

    // ���޸ġ����ǲ��ٴ�Inspector�л�ȡ������ã�Ҳ���������㼶
    private SeedSelectionUI seedSelectionUI;

    [SerializeField] private FarmUI farmUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ���ؼ��޸�����Awakeʱ��ʹ��FindObjectOfType�����������в���Ψһ��SeedSelectionUIʵ��
        // ���������Ӳ㼶��Inspector���ӣ�����ֱ�ӿɿ��ķ�ʽ
        seedSelectionUI = FindObjectOfType<SeedSelectionUI>(true); // true��ʾ��ʹ���󲻼���Ҳ���ҵ�

        if (seedSelectionUI == null)
        {
            Debug.LogError($"[FarmUIManager] ��������������FarmScene�����ж��Ҳ���������SeedSelectionUI�ű�����壡��ȷ�ϸ����ͽű��������ڳ����С�");
        }
    }

    public void ShowSeedSelectionPanel(FarmPlotController targetPlot)
    {
        if (seedSelectionUI != null)
        {
            seedSelectionUI.Open(targetPlot);
        }
        else
        {
            Debug.LogError("[FarmUIManager] �޷�������ѡ����壬��Ϊ��û�б��ҵ���");
        }
    }
}