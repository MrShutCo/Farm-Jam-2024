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
    }

    public class BloodTrait : Trait
    {
        private float[] _bloodRankMultipliers = { 0f, 0.1f, 0.3f, 0.4f, 0.75f, 1f };
        
        public override EfficiencyProfile ActOn(EfficiencyProfile profile)
        {
            profile.PullRate[EResource.Blood] += _bloodRankMultipliers[(int)_rank];
            profile.PushRate[EResource.Blood] += _bloodRankMultipliers[(int)_rank];
            return profile;
        }
    }
}