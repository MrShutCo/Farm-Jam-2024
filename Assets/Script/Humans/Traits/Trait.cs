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
        protected string Name;
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
        private float[] _rankMultipliers = { 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

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
            profile.WorkRate[EResource.Blood] += _rankMultipliers[(int)_rank];
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