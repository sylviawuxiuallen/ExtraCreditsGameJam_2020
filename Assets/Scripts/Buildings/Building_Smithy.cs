﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Building_Smithy : Building
{
    public void Start()
    {
        jobs = new TownJob[1];
        jobs[0] = new TownJob_Smith(this);
    }
}