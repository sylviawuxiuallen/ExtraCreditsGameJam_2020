using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TownJob
{
    public Building building;

    public TownResourceID consumed;
    public int consumedAmount;
    public TownResourceID produced;
    public int producedAmount;

    public float timeToFinish;

    public bool inProgress;

    public abstract void UpdateJob();
}