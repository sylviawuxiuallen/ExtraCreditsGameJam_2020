using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TownResouceID
{
    RESOURCE_WOOD,
    RESOURCE_STONE,
    RESOURCE_MEAL,
    RESOURCE_WHEAT,
    RESOURCE_FOOD,
    RESOURCE_TOOL,
    RESOURCE_BEER,
    RESOURCE_TRADE,
    RESOURCE_GLASS
}
public abstract class TownResource : MonoBehaviour
{
    public static string resourceName;
    public int amount;
}
