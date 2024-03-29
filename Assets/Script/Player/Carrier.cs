using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.Humans;
using System.Linq;
using Assets.Script.Buildings;
using Unity.VisualScripting;

public class Carrier : MonoBehaviour
{
    int maxHumans = 2;
    int maxResources = -1;

    public List<Human> CarriedHumans;
    public Dictionary<EResource, int> CarriedResources = new();
    [SerializeField] RectTransform HumanTracker;

    SoundRequest collectHuman;
    SoundRequest collectResource;
    SoundRequest dropOff;
    LiveStockBuilding stable;
    Collider2D stableCollider;
    int randIndex;


    private void Awake()
    {
        collectHuman = new SoundRequest
        {
            SoundSource = ESoundSource.Player,
            RequestingObject = gameObject,
            SoundType = ESoundType.playerCollectHuman,
            RandomizePitch = true,
            Loop = false
        };
        collectResource = new SoundRequest
        {
            SoundSource = ESoundSource.Player,
            RequestingObject = gameObject,
            SoundType = ESoundType.playerCollectResource,
            RandomizePitch = true,
            Loop = false
        };
        dropOff = new SoundRequest
        {
            SoundSource = ESoundSource.Player,
            RequestingObject = gameObject,
            SoundType = ESoundType.playerDropOff,
            RandomizePitch = true,
            Loop = false
        };


    }
    private void Start()
    {
        stable = FindObjectOfType<LiveStockBuilding>();
        stableCollider = stable.GetComponent<Collider2D>();
    }

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
            human.Deselect();
            human.anim.SetBool("IsWalking", false);
            human.anim.SetBool("IsScreaming", true);
            human.enabled = false;
            human.WildBehaviour.enabled = false;
            human.GetComponentInChildren<Renderer>().sortingOrder = 150;
            human.gameObject.GetComponent<HumanHealth>().MoveToStatusBar();
            GameManager.Instance.onCarriedHumansChange?.Invoke(CarriedHumans);
            GameManager.Instance.onPlaySound?.Invoke(collectHuman);
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
        if (maxResources > -1 && CarriedResources.Values.Sum() + amount > maxResources)
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
            GameManager.Instance.onPlaySound?.Invoke(collectResource);
            return true;
        }
    }
    public void DropOff()
    {
        randIndex = 0;
        int dropOffAmount = 0;
        for (int i = 0; i < CarriedHumans.Count; i++)
        {
            CarriedHumans[i].transform.SetParent(GameManager.Instance.HomeHumanoidParent.transform);
            CarriedHumans[i].enabled = false;
            CarriedHumans[i].enabled = true;
            CarriedHumans[i].GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
            CarriedHumans[i].transform.position = this.transform.position;

            CarriedHumans[i].StartCoroutine(CarriedHumans[i].LerpToPosition((Vector2)GetRandomPosition()));
            //CarriedHumans[i].GetComponent<HumanHealth>().SetVisiblily(false);
            dropOffAmount++;
        }
        CarriedHumans.Clear();
        foreach (var resource in CarriedResources.ToList())
        {
            if (resource.Value == 0) { continue; }
            GameManager.Instance.AddResource(resource.Key, resource.Value);
            CarriedResources[resource.Key] = 0;
            dropOffAmount++;
        }
        if (dropOffAmount > 0)
        {
            GameManager.Instance.onPlaySound?.Invoke(dropOff);
        }
        GameManager.Instance.onCarriedHumansChange?.Invoke(CarriedHumans);
        GameManager.Instance.onCarriedResourcesChange?.Invoke(CarriedResources);
    }
    Vector3 GetRandomPosition()
    {
        randIndex++;
        Random.InitState(System.DateTime.Now.Millisecond + randIndex);
        var b = stableCollider.bounds;
        var (x, y) = (Random.Range(b.min.x * .8f, b.max.x * .8f), Random.Range(b.min.y * .8f, b.max.y * .8f));

        return new Vector3(x, y, 0);
    }



    public List<Human> LoseHumans()
    {
        if (CarriedHumans.Count == 0)
        {
            return null;
        }
        List<Human> humans;
        int start = Mathf.CeilToInt(CarriedHumans.Count * .75f);
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
            int quantityToLose = Mathf.CeilToInt(resource.Value / 4);
            resourcesToRemove.Add(resource.Key, quantityToLose);
        }

        foreach (var resourceToRemove in resourcesToRemove)
        {
            CarriedResources[resourceToRemove.Key] -= resourceToRemove.Value;
        }

        return resourcesToRemove;
    }
}