using System.Collections.Generic;

namespace Assets.Script.UI
{
    public class DialogueUpgrade : DialogueText
    {
        public DialogueUpgrade()
        {
            _baseText = "What would you like to upgrade?";
            var failedText = new DialogueText("Thou doth not have enough resources to be better", null);
            Options = new List<DialogueOption>()
            {
                new("Health", () => true, new DialogueText("Thou shalt be healthier", null), failedText),
                new("Dash", () => true, new DialogueText("Thou shalt be made faster", null), failedText),
                new("Attack", () => true, new DialogueText("Thou shalt be stronger", null), failedText),
            };
        }
    }
}