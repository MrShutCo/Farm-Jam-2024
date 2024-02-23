
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private List<Stats> statsList;
    private const string statsPath = "Assets/Script/StatsAndUpgrades/Stats";

    private void OnApplicationQuit()
    {
        statsList = HelperFunctions.GetScriptableObjects<Stats>(statsPath);
        foreach (var stats in statsList)
        {
            stats.ResetAppliedUpgrades();
        }
    }
}