// FarmPlotController.cs (����������)
using UnityEngine;
using UnityEngine.EventSystems;

public class FarmPlotController : MonoBehaviour, IPointerClickHandler
{
    // ����ؿ����ϸ״̬
    private enum PlotStatus { Empty, Seeded, Watered, Growing, ReadyToHarvest }

    [Header("״̬���������Բ鿴��")]
    [SerializeField] private PlotStatus currentStatus;

    [Header("�����Ԥ�Ƽ�����")]
    [SerializeField] private SpriteRenderer cropSpriteRenderer;
   //[SerializeField] private GameObject sparkleEffectPrefab;

    [Header("������Ʒ�ʲ�")]
    [Tooltip("���ڴ˴�����'ˮ��'��ItemData�ʲ�")]
    [SerializeField] private ItemData wateringCanItem;
    [Tooltip("���ڴ˴�����'����'��ItemData�ʲ�")]
    [SerializeField] private ItemData fertilizerItem;

    // �ڲ�״̬����
    private CropData currentCrop;
    private int growthDays;
    private bool isWateredToday;
    private bool isFertilized; // ���ѻָ�����¼�Ƿ���ʩ��

    // Unity�������ڷ���
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

    // ���Ľ�����������������˵ؿ�ʱ����
    public void OnPointerClick(PointerEventData eventData)
    {
        if (HandheldItemSystem.Instance == null) return;
        ItemData heldItem = HandheldItemSystem.Instance.HeldItem;

        switch (currentStatus)
        {
            case PlotStatus.Empty:
                // ����յؿ飬������ѡ��UI
                FarmUIManager.Instance.ShowSeedSelectionPanel(this);
                break;

            case PlotStatus.Seeded:
                // ����Ѳ��ֵ�����δ��ˮ�����ֳ�ˮ��
                if (!isWateredToday && heldItem != null && heldItem.usage == ItemUsage.Tool_WateringCan) Water();
                else PlayEffect(); // ���򣬲�����Ч
                break;

            case PlotStatus.Growing:
                // ����������У����ֳַ���
                if (heldItem != null && heldItem.usage == ItemUsage.Tool_Fertilizer) Fertilize();
                else PlayEffect(); // ���򣬲�����Ч
                break;

            case PlotStatus.ReadyToHarvest:
                // ����ѳ��죬ֱ���ջ������ض����ߣ�
                Harvest();
                break;
        }
    }

    // ��ֲ����SeedSelectionUI���ã�
    public void Plant(CropData crop)
    {
        currentCrop = crop;
        growthDays = 0;
        isWateredToday = false;
        isFertilized = false; // ��ֲʱ����ʩ��״̬
        SetStatus(PlotStatus.Seeded);
        UImanager.Instance.ShowGlobalHint($"������ {currentCrop.cropName} �����ӡ�");
    }

    private void Water()
    {
        isWateredToday = true;
        UImanager.Instance.ShowGlobalHint("�����ｽ��ˮ��");
        // ��ˮ��״̬��Ϊ�����У�����Ϳ��Կ�ʼ��������
        SetStatus(PlotStatus.Growing);
    }

    // ʩ��
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

        UImanager.Instance.ShowGlobalHint("������ʩ�˷ʣ������ٶȼӿ��ˣ�");
        CheckForMaturity(); // ʩ�ʺ���������Ƿ����
    }

    // �ջ�
    private void Harvest()
    {
        int amount = Random.Range(currentCrop.minYieldAmount, currentCrop.maxYieldAmount + 1);
        InventorySystem.Instance.AddItem(currentCrop.harvestYield, amount);
        UImanager.Instance.ShowGlobalHint($"�ջ��� {currentCrop.cropName} x{amount}");
        SetStatus(PlotStatus.Empty);
    }

    // ÿ�����ʱ����
    private void OnDayEnd(int newDay)
    {
        if (currentStatus == PlotStatus.Growing)
        {
            // ������콽��ˮ����������
            if (isWateredToday)
            {
                growthDays++;
                CheckForMaturity();
            }
            else
            {
                // ���û��ˮ������ͣ��
                UImanager.Instance.ShowGlobalHint($"{currentCrop.cropName}��Ϊȱˮ��ֹͣ��������");
            }
            // ÿ����������ý�ˮ״̬���ڶ�����Ҫ���½�ˮ
            isWateredToday = false;
        }
    }

    // ����Ƿ����
    private void CheckForMaturity()
    {
        if (growthDays >= currentCrop.daysToGrow)
        {
            SetStatus(PlotStatus.ReadyToHarvest);
        }
        UpdateSprite();
    }

    // ͳһ��״̬���ú���۸���
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
    /// ��������Ч����
    /// ���޸ġ�������ͨ�����ö���ع�������������Ч��
    /// </summary>
    private void PlayEffect()
    {
        if (ParticleEffectManager.Instance != null)
        {
            // ����������󲥷���Ϊ"SparkleEffect_Prefab"����Ч
            // ע�⣺������ַ�������������Ԥ�Ƽ��ļ�����ȫһ�£�
            ParticleEffectManager.Instance.PlayEffect("SparkleEffect_Prefab", transform.position);
        }
    }
}