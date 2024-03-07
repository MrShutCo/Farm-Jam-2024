using System;
using JetBrains.Annotations;

namespace Assets.Script.Humans.Traits
{
    public enum ERank
    {
        F = 0, D, C, B, A, S
    }

    public abstract class Trait
    {
        public string Name;
        protected ERank _rank;

        public abstract EfficiencyProfile ActOn(EfficiencyProfile profile);

        public override string ToString()
        {
            return $"({_rank}) {Name}";
        }
    }

    public class ResourceTrait : Trait
    {
        private EResource _resource;
        private float[] _rankMultipliers = { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f };

        public ResourceTrait(EResource resource, ERank rank)
        {
            _rank = rank;
            _resource = resource;
            Name = resource switch
            {
                EResource.Blood => "Extra Juicy",
                EResource.Bones => "Big Boned",
                EResource.Organs => "Gutsy",
                _ => Name
            };
        }

        public override EfficiencyProfile ActOn(EfficiencyProfile profile)
        {
            profile.WorkRate[_resource] *= _rankMultipliers[(int)_rank];
            return profile;
        }
        
        public EResource GetResourceType()
        {
            return _resource;
        }
        
        public string GetRank()
        {
            return _rank.ToString();
        }
    }
}