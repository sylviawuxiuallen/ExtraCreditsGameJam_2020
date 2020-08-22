using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TownJob : MonoBehaviour
{
    public static string jobName;

    public static TownResource consumedResource;    // The resource to consume - might be null
    public static int amountConsumed;               // The amount consumed per job cycle
    public static TownResource producedResource;    // The resource the job produces
    public static float amountProduced;             // The amount produced per job cycle

    public static float wealthProduced = 0;         // The amount of wealth produced per job cycle

    public static float jobTime;    // The amount of time it takes to perform one job cycle, in seconds.

    public static Skill jobSkill;   // The skill associated with the job

    public static bool skillMaster;     // for the Guild Master job
    public static float managementBonus;    // For clerk jobs; this bonus is added to a global multiplier that decreases all villagers' jobTime
    public static float tradeBonus;     // a global bonus to prices when trading wealth for resources
    public static float birthRateBonus; // a global bonus to birth rate

    public Building jobBulding;         // the building the job is associated with

    public abstract float JobEfficiency();  // A multiplier applied to jobTime;

    public abstract bool JobCycle();    // performs one cycle of work; returns false if the job could not be done (e.g. not enough resources)

}