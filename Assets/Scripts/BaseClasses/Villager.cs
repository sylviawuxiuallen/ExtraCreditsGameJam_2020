using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;


public enum VillagerJob
{
    CHILD,
    HAULER,
    WORKER,
    IDLE
}
public class Villager : MonoBehaviour
{
    public Skill[] skills;
    public VillagerJob currentJob;
    
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
        baseCarryCapacity = 100;
        path = new NavPath();
        age = 21;
        gender = 0.5f > Random.value;
        gay = false;
        pathTo = transform.position;
        pathFrom = transform.position;
        position = map.toGridSpace(pathTo);
        pathAlpha = 0;
    }

    void Start()
    {
        InvokeRepeating("UpdateVillager", 0.0f, 0.5f);
    }

    void FixedUpdate()
    {
        if (pathAlpha > 0.95f)
        {
            if (path.path.Count == 0 || path.positionInPath == path.path.Count - 1)
            {
                //No more path.
                pathAlpha = 0;
                position = map.toGridSpace(pathTo);
                map.moveVillager(map.toGridSpace(pathFrom), map.toGridSpace(pathTo), ID);
                pathFrom = pathTo;
                path.positionInPath++;
                pathTo = map.navGrid.toWorldSpace(path.path[path.positionInPath]);
                //is new coords blocked?
                if(map.navGrid.weights[path.path[path.positionInPath].x, path.path[path.positionInPath].y] == -1)
                {
                    //blocked, recalculate path.
                    setPath(position, path.path[path.path.Count -1]);
                    pathTo = pathFrom;
                }
            }
            else
            {
                //more path, move along path.
                pathAlpha += Time.deltaTime * 5;
                transform.position = Vector3.Lerp(pathFrom, pathTo, pathAlpha);
            }
        }
    }

    public void assignWorkTask(JobWorkTask task)
    {
        workTask = task;
        isWorking = true;
        //recalculate path
        setPath(position, workTask.building.entrance);
    }

    public void assignHaulTask(JobHaulTask task)
    {
        haulTask = task;
        isWorking = false;
        //recalculate path
        setPath(position, haulTask.from.entrance);
    }

    private void UpdateVillager()
    {
        /*public struct JobWorkTask
{
    public Building building;
    public TownResource consumed;
    public TownResource produced;
    public float timeToComplete;
    public float progress;
}
         */
        //reached end of path.
        if (isWorking)
        {
            //do work task
            if (!workTask.finished)
            {
                //there actually exists a task.v

            }
        } else
        {
            //do hauling task
            if (!haulTask.finished)
            {
                //there actually exists a task.
                if(position == haulTask.from.entrance && haulTask.stage == 0)
                {
                    //am currently at the giver
                    setPath(position, haulTask.to.entrance);
                    //take resource from building.
                    
                    foreach(TownResource tr in haulTask.from.storedResources)
                    {
                        if(tr.id == haulTask.item)
                        {
                            //how many resources does the building have?
                            if (tr.amount < 100)
                            {
                                //can take all
                                carriedResource.id = haulTask.item;
                                if(haulTask.amount > tr.amount)
                                {
                                    carriedResource.amount = tr.amount;
                                } else
                                {
                                    carriedResource.amount = haulTask.amount;
                                }
                                haulTask.from.RemoveResource(haulTask.item, haulTask.amount);
                            }
                            else
                            {
                                //take a portion.
                                carriedResource.id = haulTask.item;
                                carriedResource.amount = 100;
                                haulTask.from.RemoveResource(haulTask.item, 100);
                            }
                        }
                    }
                    haulTask.stage = 1;
                }

                if(position == haulTask.to.entrance && haulTask.stage == 1)
                {
                    //am currently at the reciver
                    haulTask.to.AddResource(carriedResource);
                    haulTask.amount -= carriedResource.amount;
                    carriedResource.amount = 0;
                    if(haulTask.amount > 0)
                    {
                        haulTask.stage = 0;
                        setPath(position, haulTask.from.entrance);
                    } else
                    {
                        //task is over. it's done.
                        manager.jobFinished(this);
                        haulTask.finished = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// MULTITHREADING BULLSHIT. ALL YE WHO ENTER HERE BEWARE OF BULLSHITTERY.
    /// </summary>

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
    public void setPath(Vector2Int from, Vector2Int to)
    {
        getPathJob job = new getPathJob();
        job.from = from;
        job.to = to;
        job.path = new NavPath();
        job.grid = map.navGrid;

        JobHandle handle = job.Schedule();
        //once finished, set the path
        StartCoroutine(WaitForPathFinding(handle,job));
    }

    private IEnumerator WaitForPathFinding(JobHandle handle, getPathJob job)
    {
        yield return new WaitUntil(() => handle.IsCompleted);
        path = job.path;
    }
}
