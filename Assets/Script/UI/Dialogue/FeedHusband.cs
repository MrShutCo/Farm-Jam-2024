using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans;
using Script.Stats_and_Upgrades;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

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

            DialogueText success;
            
            // Mandatory upgrade
            if (currCost.possibleUpgradeTypes.Count == 1)
            {
                success = new DialogueAction("I can feel my power growing stronger! But my thirst beckons for more, have this reward",
                    () => Upgrade(currCost.possibleUpgradeTypes.First()));
            }
            // Choose different upgrades
            else
            {
                var options = currCost.possibleUpgradeTypes.Select(c => new DialogueOption(UpgradeDescription(c), new DialogueAction("Very well then.",
                    () => Upgrade(c))));
                success = new DialogueText("I can feel my power growing stronger! I can bestow upon you on of these options", options.ToList());
            }

            var failed = new DialogueText("Why must you waste my time Cynthia?", null);
            Options = new List<DialogueOption>()
            {
                new("Yes", currCost.CanBuy, success, failed),
                new("No", failed)
            };
        }

        string UpgradeDescription(EUpgradeType type)
        {
            return type switch
            {
                EUpgradeType.AttackPlus50 => "Attack +50",
                EUpgradeType.HealthPlus50 => "Health +50",
                EUpgradeType.CarryingCapacityPlus4 => "Carrying Capacity +4",
                _ => ""
            };
        }

        void Upgrade(EUpgradeType type)
        {
            foreach (var resource in getCurrentCost().cost)
            {
                GameManager.Instance.AddResource(resource.Resource, -resource.Amount);
            }

            switch (type)
            {
                case EUpgradeType.AttackPlus50:
                    break;
                case EUpgradeType.HealthPlus50:
                    break;
                case EUpgradeType.CarryingCapacityPlus4:
                    break;
            }

            GameManager.Instance.Stage++;
            GameManager.Instance.onGoalReached?.Invoke();
        }

        UpgradeCost getCurrentCost()
        {
            return _resourcesRequired[GameManager.Instance.Stage];
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