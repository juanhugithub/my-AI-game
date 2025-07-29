// FarmPlotController.cs (最终完整版)
using UnityEngine;
using UnityEngine.EventSystems;

public class FarmPlotController : MonoBehaviour, IPointerClickHandler
{
    // 定义地块的详细状态
    private enum PlotStatus { Empty, Seeded, Watered, Growing, ReadyToHarvest }

    [Header("状态（仅供调试查看）")]
    [SerializeField] private PlotStatus currentStatus;

    [Header("组件与预制件引用")]
    [SerializeField] private SpriteRenderer cropSpriteRenderer;
   //[SerializeField] private GameObject sparkleEffectPrefab;

    [Header("工具物品资产")]
    [Tooltip("请在此处拖入'水壶'的ItemData资产")]
    [SerializeField] private ItemData wateringCanItem;
    [Tooltip("请在此处拖入'肥料'的ItemData资产")]
    [SerializeField] private ItemData fertilizerItem;

    // 内部状态变量
    private CropData currentCrop;
    private int growthDays;
    private bool isWateredToday;
    private bool isFertilized; // 【已恢复】记录是否已施肥

    // Unity生命周期方法
    private void OnEnable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.Subscribe<int>(GameEvents.OnDayEnd, OnDayEnd);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.Unsubscribe<int>(GameEvents.OnDayEnd, OnDayEnd);
    }

    private void Start()
    {
        SetStatus(PlotStatus.Empty);
    }

    // 核心交互方法：当鼠标点击此地块时调用
    public void OnPointerClick(PointerEventData eventData)
    {
        if (HandheldItemSystem.Instance == null) return;
        ItemData heldItem = HandheldItemSystem.Instance.HeldItem;

        switch (currentStatus)
        {
            case PlotStatus.Empty:
                // 点击空地块，打开种子选择UI
                FarmUIManager.Instance.ShowSeedSelectionPanel(this);
                break;

            case PlotStatus.Seeded:
                // 如果已播种但当天未浇水，且手持水壶
                if (!isWateredToday && heldItem != null && heldItem.usage == ItemUsage.Tool_WateringCan) Water();
                else PlayEffect(); // 否则，播放特效
                break;

            case PlotStatus.Growing:
                // 如果在生长中，且手持肥料
                if (heldItem != null && heldItem.usage == ItemUsage.Tool_Fertilizer) Fertilize();
                else PlayEffect(); // 否则，播放特效
                break;

            case PlotStatus.ReadyToHarvest:
                // 如果已成熟，直接收获（无需特定工具）
                Harvest();
                break;
        }
    }

    // 种植（由SeedSelectionUI调用）
    public void Plant(CropData crop)
    {
        currentCrop = crop;
        growthDays = 0;
        isWateredToday = false;
        isFertilized = false; // 种植时重置施肥状态
        SetStatus(PlotStatus.Seeded);
        UImanager.Instance.ShowGlobalHint($"播下了 {currentCrop.cropName} 的种子。");
    }

    private void Water()
    {
        isWateredToday = true;
        UImanager.Instance.ShowGlobalHint("给作物浇了水。");
        // 浇水后，状态变为生长中，当天就可以开始计算生长
        SetStatus(PlotStatus.Growing);
    }

    // 施肥
    private void Fertilize()
    {
        isFertilized = true;
        int remainingDays = currentCrop.daysToGrow - growthDays;

        if (remainingDays <= 1)
        {
            growthDays = currentCrop.daysToGrow;
        }
        else
        {
            growthDays += Mathf.CeilToInt(remainingDays / 2f);
        }

        UImanager.Instance.ShowGlobalHint("给作物施了肥，生长速度加快了！");
        CheckForMaturity(); // 施肥后立即检查是否成熟
    }

    // 收获
    private void Harvest()
    {
        int amount = Random.Range(currentCrop.minYieldAmount, currentCrop.maxYieldAmount + 1);
        InventorySystem.Instance.AddItem(currentCrop.harvestYield, amount);
        UImanager.Instance.ShowGlobalHint($"收获了 {currentCrop.cropName} x{amount}");
        SetStatus(PlotStatus.Empty);
    }

    // 每天结束时调用
    private void OnDayEnd(int newDay)
    {
        if (currentStatus == PlotStatus.Growing)
        {
            // 如果当天浇过水，正常生长
            if (isWateredToday)
            {
                growthDays++;
                CheckForMaturity();
            }
            else
            {
                // 如果没浇水，生长停滞
                UImanager.Instance.ShowGlobalHint($"{currentCrop.cropName}因为缺水而停止了生长。");
            }
            // 每天结束，重置浇水状态，第二天需要重新浇水
            isWateredToday = false;
        }
    }

    // 检查是否成熟
    private void CheckForMaturity()
    {
        if (growthDays >= currentCrop.daysToGrow)
        {
            SetStatus(PlotStatus.ReadyToHarvest);
        }
        UpdateSprite();
    }

    // 统一的状态设置和外观更新
    private void SetStatus(PlotStatus newStatus)
    {
        currentStatus = newStatus;
        if (newStatus == PlotStatus.Empty)
        {
            currentCrop = null;
        }
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (currentStatus == PlotStatus.Empty || currentCrop == null)
        {
            cropSpriteRenderer.sprite = null;
            return;
        }

        float progress = (float)growthDays / (float)currentCrop.daysToGrow;
        int stage = Mathf.FloorToInt(progress * (currentCrop.growthSprites.Length - 1));
        stage = Mathf.Clamp(stage, 0, currentCrop.growthSprites.Length - 1);
        cropSpriteRenderer.sprite = currentCrop.growthSprites[stage];
    }

    /// <summary>
    /// 播放粒子效果。
    /// 【修改】现在它通过调用对象池管理器来播放特效。
    /// </summary>
    private void PlayEffect()
    {
        if (ParticleEffectManager.Instance != null)
        {
            // 向管理器请求播放名为"SparkleEffect_Prefab"的特效
            // 注意：这里的字符串必须与您的预制件文件名完全一致！
            ParticleEffectManager.Instance.PlayEffect("SparkleEffect_Prefab", transform.position);
        }
    }
}