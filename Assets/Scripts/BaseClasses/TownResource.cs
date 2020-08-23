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
public class TownResource
{
    public TownResource(TownResourceID id, int a)
    {
        this.id = id;
        this.amount = a;
    }

    public void deepCopy(TownResource a)
    {
        amount = a.amount;
        id = a.id;
    }

    public TownResourceID id;
    public int amount;

    public static TownResource operator+ (TownResource a, TownResource b)
    {
        TownResource c = new TownResource(a.id, a.amount);
        if(a.id == b.id)
        {
            c.amount += b.amount;
        }
        return c;
    }

    public static TownResource operator -(TownResource a, TownResource b)
    {
        TownResource c = new TownResource(a.id, a.amount);
        if (a.id == b.id)
        {
            c.amount -= b.amount;
        }
        return c;
    }
}
