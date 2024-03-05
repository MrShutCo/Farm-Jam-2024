using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;

namespace Assets.Script.UI
{
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
            var failed = new DialogueText("Why must you waste my time Cthylla?", null);
            Options = new List<DialogueOption>()
            {
                new("Yes", hasRequiredHumans, new ConsumeHumanDialogue("I can feel my power growing stronger! But my thirst beckons for more", _humansRequired), failed),
                new("No", failed)
            };
        }
        bool hasRequiredHumans()
        {
            return _humansRequired.All(human => GameManager.Instance.Carrier.CarriedHumans.Contains(human));
        }
    }
}