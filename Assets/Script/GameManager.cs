using Assets.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public int CurrTime;
    public int Money;
    public CropData SelectedCropToPlant;

    public event UnityAction onTimeStep;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        Crop.onPlantCrop += OnPlantCrop;
        Crop.onHarvestCrop += OnHarvestCrop;
    }

    private void OnDisable()
    {
        Crop.onPlantCrop -= OnPlantCrop;
        Crop.onHarvestCrop -= OnHarvestCrop;
    }

    void OnPlantCrop(CropData crop)
    {
        Debug.Log("Planted Crop");
    }

    void OnHarvestCrop(CropData crop)
    {
        Debug.Log("Harvested Crop");
    }
}