using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TownResourceID
{
    RESOURCE_WOOD,
    RESOURCE_STONE,
    RESOURCE_METAL,
    RESOURCE_WHEAT,
    RESOURCE_FOOD,
    RESOURCE_TOOL,
    RESOURCE_BEER,
    RESOURCE_TRADE,
    RESOURCE_GLASS
}
public class TownResource : MonoBehaviour
{
    public TownResource(TownResourceID id, int a)
    {
        this.id = id;
        this.amount = a;
    }

    public TownResourceID id;
    public int amount;
}
