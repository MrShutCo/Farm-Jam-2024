using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.Humans;

public class Carrier : MonoBehaviour
{
    int maxHumans = 2;
    int maxResources = 5;

    public List<Human> CarriedHumans;
    public Dictionary<EResource, int> CarriedResources;

    public bool AddCarriedHumans(Human human)
    {
        if (CarriedHumans.Count >= maxHumans)
        {
            return false;
        }
        else
        {
            CarriedHumans.Add(human);
            //put human in tendril or bag
            return true;
        }
    }
    public void RemoveCarriedHumans(Human human)
    {
        CarriedHumans.Remove(human);
    }
    public void AddCarriedResources(EResource resource, int amount)
    {
        CarriedResources[resource] += amount;
    }
}
