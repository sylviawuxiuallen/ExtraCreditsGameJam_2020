using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public enum BuildingType
    {
        TYPE_COTTAGE,
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
    public Vector2Int position;
    public Vector2Int entrance;

    public NaturalSite.NaturalSiteType NaturalSiteType; // a natural site that the building must be built on, e.g. an ore vein
    public BuildingType type;
    public VillagerProfession profession;

    public TownJob job;   // the jobs that the building allows

    public int storageCapacity;  // the amount of resources the building can store
    public List<TownResource> storedResources;    // which resources are currently stored

    public float builtPercentage = 0.0f;

    public Dictionary<TownResourceID, int> constructionCost;

    public int StoredResourcesCount()
    {
        int count = 0;

        foreach (TownResource r in storedResources)
        {
            count += r.amount;
        }

        return count;
    }

    public TownResource RemoveResource(TownResourceID resourceID, int limit)
    {
        foreach (TownResource r in storedResources)
        {
            if (r.id == resourceID)
            {
                if (r.amount < limit)
                {
                    storedResources.Remove(r);
                    return r;
                }
                else
                {
                    r.amount -= limit;
                    return new TownResource(resourceID, limit);
                }
            }
        }

        return null;
    }

    public void AddResource(TownResource resourceToStore)
    {
        bool newResource = true;
        //TODO : StoredResources is a list, no need for a for-each loop.
        foreach (TownResource r in storedResources)
        {
            if (r.id == resourceToStore.id)
            {
                r.amount += resourceToStore.amount;
                newResource = false;
            }
        }

        if (newResource)
        {
            storedResources.Add(resourceToStore);
        }
    }

}