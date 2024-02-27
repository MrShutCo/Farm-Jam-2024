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
            human.GetComponentInChildren<Renderer>().sortingOrder = 120;
            GameManager.Instance.onCarriedHumansChange?.Invoke(CarriedHumans);
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
    public void DropOff()
    {

        for (int i = 0; i < CarriedHumans.Count; i++)
        {
            CarriedHumans[i].transform.SetParent(GameManager.Instance.HomeHumanoidParent.transform);
            CarriedHumans[i].enabled = false;
            CarriedHumans[i].enabled = true;
            CarriedHumans[i].transform.position = transform.position + new Vector3(i, 0);
            CarriedHumans[i].StopAllJobs();
        }
        CarriedHumans.Clear();
        foreach (var resource in CarriedResources.ToList())
        {
            GameManager.Instance.AddResource(resource.Key, resource.Value);
            CarriedResources[resource.Key] = 0;
        }
        GameManager.Instance.onCarriedHumansChange?.Invoke(CarriedHumans);
        GameManager.Instance.onCarriedResourcesChange?.Invoke(CarriedResources);
    }
    public List<Human> LoseHumans()
    {
        List<Human> humans;
        int start = Mathf.CeilToInt(CarriedHumans.Count / 2);
        int range = CarriedHumans.Count - start;
        humans = CarriedHumans.GetRange(start, range);

        foreach (var human in humans)
        {
            CarriedHumans.Remove(human);
        }
        return humans;
    }
    public Dictionary<EResource, int> LoseResources()
    {
        Dictionary<EResource, int> resourcesToRemove = new Dictionary<EResource, int>();

        foreach (var resource in CarriedResources)
        {
            int quantityToLose = Mathf.CeilToInt(resource.Value / 2);
            resourcesToRemove.Add(resource.Key, quantityToLose);
        }

        foreach (var resourceToRemove in resourcesToRemove)
        {
            CarriedResources[resourceToRemove.Key] -= resourceToRemove.Value;
        }

        return resourcesToRemove;
    }
}