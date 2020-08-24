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
        // remove completed tasks
        // haulTasks.RemoveAll(t => t.finished);

        // check unfinished buildings
        // foreach(Building b in mapManager.buildings)
        // {

        // }

        // test task - create a task to move wood to house
        if(unassignedHaulTasks.Count == 0)
        {
            foreach(Building b in mapManager.buildings)
            {
                if(b.type == Building.BuildingType.TYPE_WAREHOUSE)
                {
                    foreach(Building b2 in mapManager.buildings)
                    {
                        if (b2.type == Building.BuildingType.TYPE_COTTAGE)
                        {
                            Debug.Log("creating task");

                            JobHaulTask newTask = new JobHaulTask();
                            newTask.from = b;
                            newTask.to = b2;
                            newTask.item = TownResourceID.RESOURCE_WOOD;
                            newTask.amount = 1;
                            newTask.stage = 0; //0 - going to from. 1 - going to to
                            newTask.finished = false;

                            unassignedHaulTasks.Add(newTask);

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
            if(!v.isWorking && !v.isHauling && unassignedHaulTasks.Count > 0)
            {
                v.assignHaulTask(unassignedHaulTasks[0]);
                unassignedHaulTasks.RemoveAt(0);
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