using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public int width;   // all buildings are rectangular
    public int height;
    protected int x;    // coordinates correspond to bottom-left corner of the building
    protected int y;

    public int woodCost; // The resource costs to create the building
    public int stoneCost;
    public int metalCost;
    public int toolCost;
    public int glassCost;

    public string locationRequired; // a natural site that the building must be built on, e.g. an ore vein

    public TownJob[] jobs;   // the jobs that the building allows

    public Sprite buildingSprite;

    public int storageCapacity;  // the amount of resources the building can store
    public List<TownResource> storedResources;    // which resources are currently stored

    public int housingCapacity;  // how many villagers the building can house

    public int StoredResourcesCount()
    {
        int count = 0;

        foreach (TownResource r in storedResources)
        {
            count += r.amount;
        }

        return count;
    }

    public void SetLocation(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public abstract float BuildingEfficiency(); // A multiplier applied to how fast workers at the building can work. Decreases when storage capacity is exceeded

    public TownResource RetrieveResources(TownResourceID resourceID, int limit)
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

    public void StoreResources(TownResource resourceToStore)
    {
        bool newResource = true;

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