using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;


public enum VillagerProfession
{
    CHILD,
    HAULER,
    LOGGER,
    MINER,
    MASON,
    FARMER,
    BAKER,
    BREWER,
    SMITH,
    ARTISAN,
    MAYOR,
    CLERK,
    TAVERNKEEP,
    TRADER,
    DOCTOR
}
public class Villager : MonoBehaviour
{
    public Skill[] skills;
    public VillagerProfession profession;
    
    public Sprite sprite;

    public int ID;
    public Vector2Int position;

    public int age;
    public bool gender;
    public bool gay;

    public TownResource carriedResource;
    public int baseCarryCapacity;

    public MapManager map;
    public TaskManager manager;

    public bool isWorking;
    public JobWorkTask workTask;
    public JobHaulTask haulTask;

    private NavPath path;
    private Vector3 pathTo;
    private Vector3 pathFrom;
    private float pathAlpha;

    Villager()
    {
        path = new NavPath();
        age = 21;
        gender = 0.5f > Random.value;
        gay = false;
        pathTo = transform.position;
        pathFrom = transform.position;
        pathAlpha = 0;
    }

    void FixedUpdate()
    {
        pathAlpha += Time.deltaTime * 5;
        transform.position = Vector3.Lerp(pathFrom, pathTo, pathAlpha);
    }

    public void assignWorkTask(JobWorkTask task)
    {
        workTask = task;
        isWorking = true;
        //recalculate path
        path = getPath(position, workTask.building.entrance);
    }

    public void assignHaulTask(JobHaulTask task)
    {
        haulTask = task;
        isWorking = false;
        //recalculate path
        path = getPath(position, haulTask.from.entrance);
    }

    private void moveAlongPath()
    {
        if(path.path.Count == 0 || path.positionInPath == path.path.Count -1)
        {
            //reached end of path.
            if(isWorking)
            {
                //do work task
                if (!workTask.Equals(null))
                {
                    //there actually exists a task.
                }
            } else
            {
                //do hauling task
                if (!haulTask.Equals(null))
                {
                    //there actually exists a task.
                    if(position == haulTask.from.entrance)
                    {
                        //go to to 
                        path = getPath(position, haulTask.to.entrance);
                        //take resource from building.
                        
                    }
                }
            }
        } else
        {
            //more path, move along path.
            if (pathAlpha > 0.95f)
            {
                //moved along path.
                pathAlpha = 0;
                pathFrom = pathTo;
                path.positionInPath++;
                pathTo = map.navGrid.toWorldSpace(path.path[path.positionInPath]);
            }
        }
    }

    struct getPathJob : IJob
    {
        public Vector2Int from;
        public Vector2Int to;
        public NavPath path;
        public NavGrid grid;

        public void Execute()
        {
            path = grid.findPath(from, to);
        }
    }
    public NavPath getPath(Vector2Int from, Vector2Int to)
    {
        getPathJob job = new getPathJob();
        job.from = from;
        job.to = to;
        job.path = new NavPath();
        job.grid = map.navGrid;

        JobHandle handle = job.Schedule();

        handle.Complete();

        return job.path;
    }
}
