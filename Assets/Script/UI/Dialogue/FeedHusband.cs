using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Script.Stats_and_Upgrades;

namespace Assets.Script.UI
{
    public class FeedHusband : DialogueText
    {
        private List<ResourceCost> _resourcesRequired = new()
        {
            new(EResource.Blood, 50),
            new(EResource.Bones, 10)
        };
        
        public FeedHusband()
        {
            _baseText = "This is the sacrifice I demand of you:\n";
            foreach (var r in _resourcesRequired)
            {
                _baseText += $"{r.Resource.ToString()} {r.Amount} \t";
            }

            var failed = new DialogueText("Why must you waste my time Cynthia?", null);
            // TODO: determine what resources are needed at each step
            Options = new List<DialogueOption>()
            {
                new("Yes", hasRequiredResources, new ConsumeResourceDialogue("I can feel my power growing stronger! But my thirst beckons for more", _resourcesRequired), failed),
                new("No", failed)
            };
        }

        bool hasRequiredResources()
        {
            return _resourcesRequired.All(resource => GameManager.Instance.Resources[resource.Resource] >= resource.Amount);
        }
    }

    public class ConsumeResourceDialogue : DialogueText
    {
        private List<ResourceCost> _resourceConsumed;

        public ConsumeResourceDialogue(string text, List<ResourceCost> resourceConsumed)
        {
            _baseText = text;
            _resourceConsumed = resourceConsumed;
        }
        
        public override void OnStart()
        {
            base.OnStart();
            foreach (var resource in _resourceConsumed)
            {
                GameManager.Instance.AddResource(resource.Resource, -resource.Amount);
            }
        }
    }
}