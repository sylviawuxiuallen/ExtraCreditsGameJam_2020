using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownJob_Farmer : TownJob
{
    public TownJob_Farmer(Building b)
    {
        building = b;
        consumed = TownResourceID.RESOURCE_WHEAT;
        consumedAmount = 0;
        produced = TownResourceID.RESOURCE_WHEAT;
        producedAmount = 10;
        timeToFinish = 10.0f;
        inProgress = false;
    }

    public override void UpdateJob()
    {
        
    }
}
