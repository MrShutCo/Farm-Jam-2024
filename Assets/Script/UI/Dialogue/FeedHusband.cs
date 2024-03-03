using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Script.Stats_and_Upgrades;
using UnityEditor;
using UnityEngine;

namespace Assets.Script.UI
{
    public class FeedHusband : DialogueText
    {
        private List<UpgradeCost> _resourcesRequired = new()
        {
            AssetDatabase.LoadAssetAtPath<UpgradeCost>("Assets/Data/Upgrades/MainUpgrade 1.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeCost>("Assets/Data/Upgrades/MainUpgrade 2.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeCost>("Assets/Data/Upgrades/MainUpgrade 3.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeCost>("Assets/Data/Upgrades/MainUpgrade 4.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeCost>("Assets/Data/Upgrades/MainUpgrade 5.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeCost>("Assets/Data/Upgrades/MainUpgrade 6.asset"),
        };

        public override void OnStart()
        {
            var currCost = getCurrentCost();
            _baseText = "This is the sacrifice I demand of you:\n";
            foreach (var r in currCost.cost)
            {
                _baseText += $"{r.Resource.ToString()} {r.Amount} \t";
            }

            var failed = new DialogueText("Why must you waste my time Cynthia?", null);
            // TODO: determine what resources are needed at each step
            Options = new List<DialogueOption>()
            {
                new("Yes", currCost.CanBuy, new ConsumeResourceDialogue("I can feel my power growing stronger! But my thirst beckons for more", currCost), failed),
                new("No", failed)
            };
        }

        UpgradeCost getCurrentCost()
        {
            return _resourcesRequired[GameManager.Instance.Stage];
        }
    }

    public class ConsumeResourceDialogue : DialogueText
    {
        private UpgradeCost _resourcesConsumed;

        public ConsumeResourceDialogue(string text, UpgradeCost resourceConsumed)
        {
            _baseText = text;
            _resourcesConsumed = resourceConsumed;
        }
        
        public override void OnStart()
        {
            base.OnStart();
            foreach (var resource in _resourcesConsumed.cost)
            {
                GameManager.Instance.AddResource(resource.Resource, -resource.Amount);
            }
        }
    }
}