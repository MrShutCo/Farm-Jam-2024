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
                new("Health", () => true, new DialogueAction("Thou shalt be healthier", UpgradeHealth), failedText),
                new("Dash", () => true, new DialogueAction("Thou shalt be made faster", UpgradeDash), failedText),
                new("Attack", () => true, new DialogueAction("Thou shalt be stronger", UpgradeAttack), failedText),
                new("More Farm Land", () => true, new DialogueAction("And so there shall be more farmland", UpgradeFarmLand), failedText)
            };
        }

        private void UpgradeFarmLand()
        {
            
        }

        private void UpgradeHealth()
        {
            
        }

        private void UpgradeDash()
        {
            
        }

        private void UpgradeAttack()
        {
            
        }
    }
}