using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public static Skill[] skills;
    public static TownJob job;

    public Sprite sprite;

    public int x;
    public int y;

    public TownResource[] carriedResources;
    public int baseCarryCapacity;

    public int CarryCapacity()
    {
        return 0;
    }

    public TownResource GiveResources(string rName, int amount)
    {
        return null;
    }

    public bool TakeResources(TownResource r)
    {
        return false;
    }
}
