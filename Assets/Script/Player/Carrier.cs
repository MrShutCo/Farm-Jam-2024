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
    public Dictionary<EResource, int> CarriedResources = new();

    [SerializeField] RectTransform HumanTracker;
    [SerializeField] RectTransform ResourceTracker;

    public void SetCarryCapacity(int humans, int resources)
    {
        maxHumans = humans;
        maxResources = resources;
    }

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
            human.WildBehaviour.enabled = false;
            human.transform.SetParent(HumanTracker);
            OrganizeHumans();
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
        if (CarriedResources.Values.Sum() + amount > maxResources)
        {
            return false;
        }
        else
        {
            if (CarriedResources.ContainsKey(resource))
            {
                CarriedResources[resource] += amount;
            }
            else
            {
                CarriedResources.Add(resource, amount);
            }
            GameManager.Instance.onCarriedResourcesChange?.Invoke(CarriedResources);
            return true;
        }
    }
    void OrganizeHumans()
    {
        Vector2 humanTrackerPos = HumanTracker.position;
        //organize humans on tracker
        for (int i = 0; i < CarriedHumans.Count; i++)
        {
            CarriedHumans[i].transform.position = humanTrackerPos + new Vector2(i, .5f);
        }
    }
    public void DropOff()
    {

        for (int i = 0; i < CarriedHumans.Count; i++)
        {
            CarriedHumans[i].transform.SetParent(null);
            CarriedHumans[i].enabled = true;
            CarriedHumans[i].ChangeLocation(true);
            CarriedHumans[i].ChangeLocation(true);
            CarriedHumans[i].transform.position = transform.position + new Vector3(i, 0);
        }
        CarriedHumans.Clear();
        foreach (var resource in CarriedResources)
        {
            GameManager.Instance.AddResource(resource.Key, resource.Value);
        }
        CarriedResources.Clear();
        GameManager.Instance.onCarriedHumansChange?.Invoke(CarriedHumans);
        GameManager.Instance.onCarriedResourcesChange?.Invoke(CarriedResources);
    }
}
