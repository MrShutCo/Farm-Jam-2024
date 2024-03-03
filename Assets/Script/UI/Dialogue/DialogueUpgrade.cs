using System.Collections.Generic;
using Script.Stats_and_Upgrades;

namespace Assets.Script.UI
{
    public class DialogueUpgrade : DialogueText
    {
        private List<UpgradeCost> upgradeStages;
        
        public DialogueUpgrade()
        {
            _baseText = "What would you like to upgrade?";
            var failedText = new DialogueText("Thou doth not have enough resources to be better", null);
            Options = new List<DialogueOption>()
            {
                new("Sacrifice", () => true, new DialogueAction("Thou shalt be healthier", UpgradeHealth), failedText),
                new("Dash", () => true, new DialogueAction("Thou shalt be made faster", UpgradeDash), failedText)
            };
        }

        
        
        public override void OnStart()
        {
           
                _baseText = "This is the sacrifice I demand of you:\n";
            /*foreach (var r in _resourcesRequired)
            {
                _baseText += $"{r.Resource.ToString()} {r.Amount} \t";
            }*/
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