using System.Collections;
using System.Collections.Generic;
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
}

public struct JobWorkTask
{
    public Building building;
    public TownResourceID consumed;
    public TownResourceID produced;
    public int amountConsumed;
    public int amountProduced;
    public float timeToComplete;
    public float progress;
}

public struct JobHaulTask
{
    public Building from;
    public Building to;
    public TownResourceID item;
    public int amount;
}