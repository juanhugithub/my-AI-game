// FarmPlotController.cs
using UnityEngine;
using System.Collections;

public class FarmPlotController : MonoBehaviour
{
    private enum PlotStatus { Empty, Seeded, Growing, ReadyToHarvest }
    private PlotStatus currentStatus;

    [SerializeField] private SpriteRenderer cropSpriteRenderer;
    private CropData currentCrop;
    private float growthTimer;

    // ��������TownScene�з���һ��ũ���ָ��������ֲ������
    [SerializeField] private CropData availableCropToPlant;

    private void Start()
    {
        currentStatus = PlotStatus.Empty;
    }

    // �����ũ�ｻ������Ҫ����
    public void Interact()
    {
        switch (currentStatus)
        {
            case PlotStatus.Empty:
                Plant(availableCropToPlant);
                break;
            case PlotStatus.ReadyToHarvest:
                Harvest();
                break;
        }
    }

    private void Plant(CropData crop)
    {
        if (crop == null) return;

        currentCrop = crop;
        currentStatus = PlotStatus.Seeded;
        growthTimer = 0f;
        cropSpriteRenderer.sprite = currentCrop.growthSprites[0];
        StartCoroutine(GrowCrop());
        Debug.Log($"����ֲ {currentCrop.cropName}");
    }

    private IEnumerator GrowCrop()
    {
        currentStatus = PlotStatus.Growing;
        float timePerStage = currentCrop.growthTimeInSeconds / (currentCrop.growthSprites.Length - 1);

        for (int i = 1; i < currentCrop.growthSprites.Length; i++)
        {
            yield return new WaitForSeconds(timePerStage);
            cropSpriteRenderer.sprite = currentCrop.growthSprites[i];
        }

        currentStatus = PlotStatus.ReadyToHarvest;
        Debug.Log($"{currentCrop.cropName} �ѳ���!");
    }

    private void Harvest()
    {
        InventorySystem.Instance.AddItem(currentCrop.harvestYield, currentCrop.yieldAmount);
        UImanager.Instance.ShowGlobalHint($"�ջ��� {currentCrop.cropName} x{currentCrop.yieldAmount}");

        // ����ũ��״̬
        currentCrop = null;
        cropSpriteRenderer.sprite = null;
        currentStatus = PlotStatus.Empty;
    }
}