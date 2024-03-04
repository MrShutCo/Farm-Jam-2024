using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Script.Buildings;
using Script.Stats_and_Upgrades;
using UnityEditor;

namespace Assets.Script.UI
{
    public class DialogueUpgrade : DialogueText
    {
        private List<UpgradeCost> _resourcesRequired;
        
        public DialogueUpgrade(List<UpgradeCost> resourcesRequired)
        {
            _resourcesRequired = resourcesRequired;
            _baseText = "What would you like to upgrade?";
            var failedText = new DialogueText("Thou doth not have enough resources to upgrade", null);
            Options = new List<DialogueOption>()
            {
                new("Blood Building +", () => CanAfford(EResource.Blood), new DialogueAction("May the blood of humans flow more easily",
                    () => UpgradeResource(EResource.Blood)), failedText),
                new("Bone Building +", () => CanAfford(EResource.Bones), new DialogueAction("These foolish humans are really boned now", 
                    () => UpgradeResource(EResource.Bones)), failedText),
                new("Organ Building +", () => CanAfford(EResource.Organs), new DialogueAction("Guts upgraded", 
                    () => UpgradeResource(EResource.Organs)), failedText)
            };
        }
        
        public override void OnStart()
        {
            //_baseText = "This is the sacrifice I demand of you:\n";
            /*foreach (var r in _resourcesRequired)
            {
                _baseText += $"{r.Resource.ToString()} {r.Amount} \t";
            }*/
        }

        bool CanAfford(EResource type)
        {
            var cost = _resourcesRequired[GameManager.Instance.BaseBuildLevel[type]];
            return GameManager.Instance.CanAfford(cost);
        }
        
        private void UpgradeResource(EResource resource)
        {
            GameManager.Instance.SubtractUpgradeCost(_resourcesRequired[GameManager.Instance.BaseBuildLevel[resource]]);
            GameManager.Instance.BaseBuildLevel[resource]++;

            foreach (var b in GameManager.Instance.Buildings)
            {
                if (b is BodyPartBuilding building && building.buildingData.resource == resource)
                {
                    building.Upgrade();
                }
            }
            
            
        }
    }
}