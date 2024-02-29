using System.Collections.Generic;

namespace Assets.Script.Humans
{
    public class EfficiencyProfile
    {
        public Dictionary<EResource, float> PullRate = new()
        {
            {EResource.Blood, 1f},
            {EResource.Bones, 1f},
            {EResource.Organs, 1f},
            {EResource.Food, 1f}
        };
        public Dictionary<EResource, float> PushRate = new()
        {
            {EResource.Blood, 1f},
            {EResource.Bones, 1f},
            {EResource.Organs, 1f},
            {EResource.Food, 1f}
        };

        public float SpeedMultiplier = 1;
        public float HealthMultiplier = 1;
        public float AttackRateMultiplier = 1;
    }
}