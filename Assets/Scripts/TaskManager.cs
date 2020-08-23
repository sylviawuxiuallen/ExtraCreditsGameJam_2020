using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Manages all the villagers and assigns them tasks to complete.
/// </summary>
public class TaskManager : MonoBehaviour
{
    List<Building> buildings;
    List<Villager> villagers;
    List<JobHaulTask> haulTasks;
    List<JobWorkTask> workTasks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Villagers call this when they have finished their task
    /// </summary>
    /// <param name="villager"></param>
    public void jobFinished(Villager villager)
    {
        villager.currentJob = VillagerJob.IDLE;
    }
}

public struct JobWorkTask
{
    public Building building;
    public TownResource consumed;
    public TownResource produced;
    public float timeToComplete;
    public float progress;
    public bool finished;
}

public struct JobHaulTask
{
    public Building from;
    public Building to;
    public TownResourceID item;
    public int amount;
    public int stage; //0 - going to from. 1 - going to to
    public bool finished;
}