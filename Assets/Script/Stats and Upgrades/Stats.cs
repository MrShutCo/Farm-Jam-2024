using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;
using ScriptableDictionaries;

[System.Serializable]
[CreateAssetMenu(fileName = "Stats", menuName = "Upgrades/Stats", order = 1)]
public class Stats : ScriptableObject
{
    public ScriptableDictionary<EStat, float> instanceStats;
    public ScriptableDictionary<EStat, float> stats;
    private List<StatsUpgrade> appliedUpgrades = new List<StatsUpgrade>();

    private void OnEnable()
    {

    }
    public float GetStat(EStat stat)
    {
        if (instanceStats.TryGetValue(stat, out var instanceValue))
            return GetUpgradedValue(stat, instanceValue);
        else if (stats.TryGetValue(stat, out float value))
            return GetUpgradedValue(stat, value);
        else
        {
            Debug.LogError($"No stat found for {stat} on {this.name}");
            return 0;
        }
    }
    public void UnlockUpgrade(StatsUpgrade upgrade)
    {
        if (!appliedUpgrades.Contains(upgrade))
        {
            appliedUpgrades.Add(upgrade);
        }
    }
    private float GetUpgradedValue(EStat stat, float baseValue)
    {
        foreach (var upgrade in appliedUpgrades)
        {
            if (!upgrade.upgradeToApply.TryGetValue(stat, out var upgradeValue))
                continue;
            if (upgrade.isPercentageUpgrade)
            {
                baseValue *= (upgradeValue / 100f) + 1f;
            }
            else
                baseValue += upgradeValue;
        }
        return baseValue;
    }
    public void ResetAppliedUpgrades()
    {
        appliedUpgrades.Clear();
    }
}
