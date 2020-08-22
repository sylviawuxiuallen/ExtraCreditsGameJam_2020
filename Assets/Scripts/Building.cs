using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public static int width;   // all buildings are rectangular
    public static int height;
    protected int x;    // coordinates correspond to bottom-left corner of the building
    protected int y;

    public static int woodCost; // The resource costs to create the building
    public static int stoneCost;
    public static int metalCost;
    public static int toolCost;
    public static int glassCost;

    public string locationRequired; // a natural site that the building must be built on, e.g. an ore vein

    public static TownJob[] jobs;   // the jobs that the building allows

    public Sprite buildingSprite;

    public static int storageCapacity;  // the amount of resources the building can store
    public TownResource[] storedResources;    // which resources are currently stored

    public static int housingCapacity;  // how many villagers the building can house

    public int StoredResourcesCount()
    {
        int count = 0;
        
        foreach(TownResource r in storedResources)
        {
            count += r.amount;
        }

        return count;
    }

    public abstract float BuildingEfficiency(); // A multiplier applied to how fast workers at the building can work. Decreases when storage capacity is exceeded

    public abstract TownResource RetrieveResources(TownResouceID resouceID, int limit);

    public abstract void StoreResources(TownResource r);

}