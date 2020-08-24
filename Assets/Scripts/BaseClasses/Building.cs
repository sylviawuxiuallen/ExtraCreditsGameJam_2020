using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public abstract class Building : MonoBehaviour
{
    public enum BuildingType
    {
        TYPE_COTTAGE,
        TYPE_WAREHOUSE,
        TYPE_LUMBER_MILL,
        TYPE_QUARRY,
        TYPE_MINE,
        TYPE_FARM,
        TYPE_BAKERY,
        TYPE_BREWERY,
        TYPE_SMITHY,
        TYPE_WORKSHOP,
        TYPE_TOWN_HALL,
        TYPE_TAVERN,
        TYPE_MARKET,
        TYPE_APARTMENT,
        TYPE_CLINIC,
        TYPE_GUILD
    }

    public int width;   // all buildings are rectangular
    public int height;
    public Vector2Int position; // bottom-left corner
    public Vector2Int entrance; // relative to position

    bool underConstruction;
    public bool isConstructed;
    public float builtPercentage = 0.0f;
    public float constructionTime;  // amount of time, in seconds, required to create the building

    public NaturalSite.NaturalSiteType NaturalSiteType; // a natural site that the building must be built on, e.g. an ore vein
    public BuildingType type;

    public TownJob[] jobs;   // the jobs that the building allows

    public int storageCapacity;  // the amount of resources the building can store
    public Dictionary<TownResourceID, int> storedResources;    // which resources are currently stored

    public int housingCapacity;

    // public Dictionary<TownResourceID, int> constructionCost;

    public int woodCost;
    public int stoneCost;
    public int metalCost;
    public int toolCost;
    public int glassCost;

    public ResourceTracker resourceTracker;

    private void Start()
    {
        InitializeStoredResources();
    }

    private void Update()
    {
        if(!isConstructed)
        {
            builtPercentage += 100.0f * Time.deltaTime / constructionTime;
            if (builtPercentage >= 100.0f)
            {
                isConstructed = true;
                Debug.Log("Construction completed!");
            }
        }
    }
    public Dictionary<TownResourceID, int> ConstructionCost()
    {
        Dictionary<TownResourceID, int> c = new Dictionary<TownResourceID, int>();

        c.Add(TownResourceID.RESOURCE_WOOD, woodCost);
        c.Add(TownResourceID.RESOURCE_STONE, stoneCost);
        c.Add(TownResourceID.RESOURCE_METAL, metalCost);
        c.Add(TownResourceID.RESOURCE_TOOL, toolCost);
        c.Add(TownResourceID.RESOURCE_GLASS, glassCost);

        return c;
    }

    public int StoredResourcesCount()
    {
        int count = 0;

        foreach (KeyValuePair<TownResourceID, int> r in storedResources)
        {
            count += r.Value;
        }

        return count;
    }

    public int RemoveResource(TownResourceID resourceID, int amountWanted)
    {
        // Tries to retrieve the resources requested. If there's not enough, return as much as possible

        if (storedResources[resourceID] >= amountWanted)
        {
            storedResources[resourceID] -= amountWanted;
            return amountWanted;
        }
        else
        {
            int amountRemoved = storedResources[resourceID];
            storedResources[resourceID] = 0;
            return amountRemoved;
        }
        
    }

    public void AddResource(TownResourceID resourceID, int amountToStore)
    {
        InitializeStoredResources();
        storedResources[resourceID] += amountToStore;
    }

    public void InitializeStoredResources()
    {
        // This is put in a separate function because in some cases, AddResource may be called before Start,
        // requiring storedResources to be initialized at that time.
        if (storedResources == null)
        {
            storedResources = new Dictionary<TownResourceID, int>();
            foreach (TownResourceID r in System.Enum.GetValues(typeof(TownResourceID)))
            {
                storedResources.Add(r, 0);
            }
        }
    }

    public bool TryStartConstruction()
    {
        foreach (TownResourceID r in System.Enum.GetValues(typeof(TownResourceID)))
        {
            if (storedResources[r] < ConstructionCost()[r])
            {
                return false;
            }
        }

        foreach (TownResourceID r in System.Enum.GetValues(typeof(TownResourceID)))
        {
            storedResources[r] -= ConstructionCost()[r];

        }
        return true;
    }

    public Dictionary<TownResourceID, int> ResourcesNeeded()
    {


        return null;
    }
}