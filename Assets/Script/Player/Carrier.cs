using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.Humans;
using System.Linq;

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
            human.enabled = false;
            human.transform.SetParent(transform);
            GameManager.Instance.onCarriedHumansChange?.Invoke(CarriedHumans);
            //put human in tendril or bag
            return true;
        }
    }
    public void RemoveCarriedHumans(Human human)
    {
        CarriedHumans.Remove(human);
        human.enabled = true;
        human.transform.SetParent(null);
    }
    public bool AddCarriedResources(EResource resource, int amount)
    {
        //if total resources is greater than max resources
        if (CarriedResources.Values.Sum() + amount > maxResources)
        {
            return false;
        }
        else
        {
            CarriedResources[resource] += amount;
            GameManager.Instance.onCarriedResourcesChange?.Invoke(CarriedResources);
            return true;
        }
    }
}
