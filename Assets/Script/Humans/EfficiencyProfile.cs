using System.Collections.Generic;

namespace Assets.Script.Humans
{
    public class EfficiencyProfile 
    {
        public Dictionary<EResource, float> WorkRate = new()
        {
            {EResource.Blood, 10f/6f},
            {EResource.Bones, 10f/6f},
            {EResource.Organs, 10f/6f},
            {EResource.Food, 10f/6f}
        };
        public float SpeedMultiplier = 1;
        public float HealthMultiplier = 1;
        public float AttackRateMultiplier = 1;
    }
}