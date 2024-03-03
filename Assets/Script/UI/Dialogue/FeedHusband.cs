using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Unity.VisualScripting;

namespace Assets.Script.UI
{
    public class FeedHusband : DialogueText
    {
        private List<(EResource resource, int amount)> _resourcesRequired = new()
        {
            new(EResource.Blood, 50),
            new(EResource.Bones, 10)
        };

        public FeedHusband()
        {
            _baseText = "This is the sacrifice I demand of you:\n";
            foreach (var r in _resourcesRequired)
            {
                _baseText += $"{r.resource.ToString()} {r.amount} \t";
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
            GameManager.Instance.onGoalReached.Invoke();
            return _resourcesRequired.All(resource => GameManager.Instance.Resources[resource.resource] >= resource.amount);
        }
    }
    public class SacrificeHuman : DialogueText
    {
        private List<Human> _humansRequired = new(){
            new Human(),
            new Human()
        };
        public SacrificeHuman()
        {
            _baseText = "This is the sacrifice I demand of you:\n";
            foreach (var h in _humansRequired)
            {
                _baseText += $"{h.Name} \t";
            }
            var failed = new DialogueText("Why must you waste my time Cynthia?", null);
            Options = new List<DialogueOption>()
        {
            new("Yes", hasRequiredHumans, new ConsumeHumanDialogue("I can feel my power growing stronger! But my thirst beckons for more", _humansRequired), failed),
            new("No", failed)
        };
        }
        bool hasRequiredHumans()
        {
            GameManager.Instance.onGoalReached.Invoke();
            return _humansRequired.All(human => GameManager.Instance.Carrier.CarriedHumans.Contains(human));
        }
    }


    public class ConsumeResourceDialogue : DialogueText
    {
        private List<(EResource resource, int amount)> _resourceConsumed;

        public ConsumeResourceDialogue(string text, List<(EResource resource, int amount)> resourceConsumed)
        {
            _baseText = text;
            _resourceConsumed = resourceConsumed;
        }

        public override void OnStart()
        {
            base.OnStart();
            foreach (var resource in _resourceConsumed)
            {
                GameManager.Instance.AddResource(resource.resource, -resource.amount);
            }
        }
    }

    public class ConsumeHumanDialogue : DialogueText
    {
        private List<Human> _humansConsumed;

        public ConsumeHumanDialogue(string text, List<Human> humansConsumed)
        {
            _baseText = text;
            _humansConsumed = humansConsumed;
        }

        public override void OnStart()
        {
            base.OnStart();
            foreach (var human in _humansConsumed)
            {
                human.GetComponent<HealthBase>().TakeDamage(human.GetComponent<HealthBase>().MaxHealth);
            }
        }
    }
}