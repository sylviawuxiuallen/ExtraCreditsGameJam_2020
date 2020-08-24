using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownJob_Miner : TownJob
{
    public TownJob_Miner(Building b)
    {
        building = b;
        consumed = TownResourceID.RESOURCE_METAL;
        consumedAmount = 0;
        produced = TownResourceID.RESOURCE_METAL;
        producedAmount = 10;
        timeToFinish = 10.0f;
        inProgress = false;
    }

    public override void UpdateJob()
    {
        
    }
}
