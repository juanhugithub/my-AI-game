// FarmUIManager.cs (终极健壮版 V2 - 使用全局查找)
using UnityEngine;

public class FarmUIManager : MonoBehaviour
{
    public static FarmUIManager Instance { get; private set; }

    // 【修改】我们不再从Inspector中获取这个引用，也不再依赖层级
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

        // 【关键修复】在Awake时，使用FindObjectOfType在整个场景中查找唯一的SeedSelectionUI实例
        // 这样做无视层级和Inspector链接，是最直接可靠的方式
        seedSelectionUI = FindObjectOfType<SeedSelectionUI>(true); // true表示即使对象不激活也能找到

        if (seedSelectionUI == null)
        {
            Debug.LogError($"[FarmUIManager] 致命错误：在整个FarmScene场景中都找不到挂载了SeedSelectionUI脚本的面板！请确认该面板和脚本都存在于场景中。");
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
            Debug.LogError("[FarmUIManager] 无法打开种子选择面板，因为它没有被找到！");
        }
    }
}