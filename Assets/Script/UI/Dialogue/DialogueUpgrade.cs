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
            
        }
        
        public override void OnStart()
        {
            var failedText = new DialogueText("Thou doth not have enough resources to upgrade", null);

            Options = new List<DialogueOption>();
            if (GameManager.Instance.BaseBuildLevel[EResource.Blood] < 2) 
                Options.Add( new($"Blood+ ({GetResourcesRequiredText(EResource.Blood)})", () => CanAfford(EResource.Blood), new DialogueAction("May the blood of humans flow more easily",
                    () => UpgradeResource(EResource.Blood)), failedText));
            if (GameManager.Instance.BaseBuildLevel[EResource.Bones] < 2) 
                Options.Add(new($"Bone+  ({GetResourcesRequiredText(EResource.Bones)})", () => CanAfford(EResource.Bones), new DialogueAction("These foolish humans are really boned now", 
                    () => UpgradeResource(EResource.Bones)), failedText));
            if (GameManager.Instance.BaseBuildLevel[EResource.Organs] < 2)
                Options.Add(new($"Organ+ ({GetResourcesRequiredText(EResource.Organs)})", () => CanAfford(EResource.Organs), new DialogueAction("Guts upgraded", 
                    () => UpgradeResource(EResource.Organs)), failedText));


            if (Options.Count == 0)
            {
                Options.Add(new DialogueOption("Nothing (all buildings have been upgraded to max)", null));
            }
        }

        string GetResourcesRequiredText(EResource type)
        {
            var cost = _resourcesRequired[GameManager.Instance.BaseBuildLevel[type]];
            var s = "";
            foreach (var r in cost.cost)
            {
                s += $"{r.Resource.ToString()}  {r.Amount} ";
            } 
            return s.Trim();
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