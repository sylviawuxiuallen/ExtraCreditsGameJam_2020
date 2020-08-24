using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Manages all the villagers and assigns them tasks to complete.
/// </summary>
public class TaskManager : MonoBehaviour
{
    // List<Building> buildings;
    // List<Villager> villagers;
    List<JobHaulTask> unassignedHaulTasks;
    List<JobWorkTask> unassignedWorkTasks;

    MapManager mapManager;

    // Start is called before the first frame update
    void Start()
    {
        unassignedHaulTasks = new List<JobHaulTask>();
        unassignedWorkTasks = new List<JobWorkTask>();

        mapManager = this.gameObject.GetComponent<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // check unfinished buildings
        foreach(Building b in mapManager.buildings)
        {
            if (!b.isConstructed && !b.underConstruction && !b.resourcesOrdered)
            {
                b.InitializeStoredResources();
                Dictionary<TownResourceID, int> resourcesToGet = b.ResourcesNeededToBuild();

                foreach(KeyValuePair<TownResourceID, int> r in resourcesToGet)
                {
                    if(r.Value > 0)
                    {
                        JobHaulTask newTask = new JobHaulTask();
                        newTask.from = gameObject.GetComponent<ResourceTracker>().FindResourceStock(r);
                        newTask.to = b;
                        newTask.item = r.Key;
                        newTask.amount = r.Value;
                        newTask.stage = 0; //0 - going to from. 1 - going to to
                        newTask.finished = false;

                        unassignedHaulTasks.Add(newTask);
                    }
                }

                b.resourcesOrdered = true;
            } else if(b.isConstructed)
            {
                if(b.jobs != null)
                {
                    foreach(TownJob j in b.jobs)
                    {
                        if(!j.inProgress)
                        {
                            JobWorkTask newTask = new JobWorkTask();
                            newTask.job = j;
                            newTask.building = b;
                            newTask.consumed = j.consumed;
                            newTask.consumedAmount = j.consumedAmount;
                            newTask.produced = j.produced;
                            newTask.producedAmount = j.producedAmount;
                            newTask.timeToComplete = j.timeToFinish;

                            j.inProgress = true;

                            unassignedWorkTasks.Add(newTask);
                        }
                    }
                }
            }
        }

        AssignTasks();
    }

    public void AssignTasks()
    {
        foreach(Villager v in mapManager.villagers)
        {
            if(!v.isWorking && !v.isHauling)
            {
                if (unassignedHaulTasks.Count > 0)
                {
                    v.assignHaulTask(unassignedHaulTasks[0]);
                    unassignedHaulTasks.RemoveAt(0);
                } else if(unassignedWorkTasks.Count > 0)
                {
                    v.assignWorkTask(unassignedWorkTasks[0]);
                    unassignedWorkTasks.RemoveAt(0);
                }
            }
        }
    }

    /// <summary>
    /// Villagers call this when they have finished their task
    /// </summary>
    /// <param name="villager"></param>
    public void haulTaskFinished(Villager villager, JobHaulTask task)
    {
        villager.currentJob = VillagerJob.IDLE;
    }
}

public struct JobWorkTask
{
    public TownJob job;
    public Building building;
    public TownResourceID consumed;
    public int consumedAmount;
    public TownResourceID produced;
    public int producedAmount;
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