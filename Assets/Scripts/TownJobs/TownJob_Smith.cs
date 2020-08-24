using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownJob_Smith : TownJob
{
    public TownJob_Smith(Building b)
    {
        building = b;
        consumed = TownResourceID.RESOURCE_METAL;
        consumedAmount = 5;
        produced = TownResourceID.RESOURCE_TOOL;
        producedAmount = 10;
        timeToFinish = 10.0f;
        inProgress = false;
    }

    public override void UpdateJob()
    {
        
    }
}
