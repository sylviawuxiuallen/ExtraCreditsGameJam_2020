using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownJob_Lumber : TownJob
{
    public TownJob_Lumber(Building b)
    {
        building = b;
        consumed = TownResourceID.RESOURCE_WOOD;
        consumedAmount = 0;
        produced = TownResourceID.RESOURCE_WOOD;
        producedAmount = 10;
        timeToFinish = 10.0f;
        inProgress = false;
    }

    public override void UpdateJob()
    {
        
    }
}
