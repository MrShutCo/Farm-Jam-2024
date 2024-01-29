using System;
using System.Collections;
using UnityEngine;

namespace Assets.Script.Buildings
{
    public class FoodFarm : ResourceBuilding 
    {
        public Action onStageUp;

        int CurrentStage;
        int MaxStage;

        [SerializeField] Sprite[] Stages;
        SpriteRenderer renderer;

        public void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void IncreaseStage()
        {
            CurrentStage = Mathf.Max(CurrentStage + 1, MaxStage);
            renderer.sprite = Stages[CurrentStage];
            onStageUp?.Invoke();
        }

        public void Update()
        {
            base.Update();
        }
    }
}