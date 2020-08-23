using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TownJob
{
    public Building building;
    public abstract void UpdateJob();
}