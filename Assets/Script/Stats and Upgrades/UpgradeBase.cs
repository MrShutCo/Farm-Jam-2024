using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;
using ScriptableDictionaries;

[System.Serializable]
public abstract class UpgradeBase : ScriptableObject
{
    public Sprite icon { get; private set; }
    public string upgradeName { get; private set; }
    public string description { get; private set; }
    public EResource cost;

    public abstract void ApplyUpgrade();
}

[System.Serializable]
[CreateAssetMenu(fileName = "StatsUpgrade", menuName = "Upgrades/StatsUpgrade", order = 1)]
public class StatsUpgrade : UpgradeBase
{
    public List<Stats> unitsToUpgrade = new List<Stats>();
    public ScriptableDictionary<EStat, float> upgradeToApply;
    public bool isPercentageUpgrade = false;
    private void OnEnable()
    {

    }
    public override void ApplyUpgrade()
    {
        foreach (var unitToUpgrade in unitsToUpgrade)
        {
            unitToUpgrade.UnlockUpgrade(this);
        }
    }
}

