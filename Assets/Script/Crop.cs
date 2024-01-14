using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crop : MonoBehaviour
{
    private CropData curCrop;
    private int plantTime;
    private int timeSinceLastWatered;
    bool tilled;

    public SpriteRenderer sr;

    public static event UnityAction<CropData> onPlantCrop;
    public static event UnityAction<CropData> onHarvestCrop;


    public bool CanHarvest() => CropProgress() >= curCrop.TimeToHarvest;
    public void Water() => timeSinceLastWatered = 0;
    public void Harvest()
    {
        if (CanHarvest())
        {
            onHarvestCrop?.Invoke(curCrop);
            Destroy(gameObject);
        }
    }

    public void Interact()
    {
        if (!tilled) Till();
        
    }

    public void Plant(CropData crop)
    {
        curCrop = crop;
        plantTime = GameManager.Instance.CurrTime;
        timeSinceLastWatered = 1;
        UpdateCropSprite();
        onPlantCrop?.Invoke(crop);
    }

    public void TimeStep()
    {
        timeSinceLastWatered++;
        if (timeSinceLastWatered > 3) Destroy(gameObject);
        UpdateCropSprite();
    }
    
    void UpdateCropSprite()
    {
        int cropProgress = CropProgress();
        if (cropProgress < curCrop.TimeToHarvest)
            sr.sprite = curCrop.growProgressSprites[cropProgress];
        else
            sr.sprite = curCrop.readyToHarvestSprite;
    }

    void Till()
    {
        tilled = true;
        sr.sprite = curCrop.growProgressSprites[0];
    }

    int CropProgress() => GameManager.Instance.CurrTime - plantTime;

    // Update is called once per frame
    void Update()
    {
        
    }
}
