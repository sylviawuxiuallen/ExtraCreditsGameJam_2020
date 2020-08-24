using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownJob_Stonecutter : TownJob
{
    public TownJob_Stonecutter(Building b)
    {
        building = b;
        consumed = TownResourceID.RESOURCE_STONE;
        consumedAmount = 0;
        produced = TownResourceID.RESOURCE_STONE;
        producedAmount = 10;
        timeToFinish = 10.0f;
        inProgress = false;
    }

    public override void UpdateJob()
    {
        
    }
}
