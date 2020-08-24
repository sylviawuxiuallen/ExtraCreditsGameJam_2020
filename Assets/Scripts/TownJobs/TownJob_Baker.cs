using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownJob_Baker : TownJob
{
    public TownJob_Baker(Building b)
    {
        building = b;
        consumed = TownResourceID.RESOURCE_WHEAT;
        consumedAmount = 10;
        produced = TownResourceID.RESOURCE_FOOD;
        producedAmount = 10;
        timeToFinish = 10.0f;
        inProgress = false;
    }

    public override void UpdateJob()
    {
        
    }
}
