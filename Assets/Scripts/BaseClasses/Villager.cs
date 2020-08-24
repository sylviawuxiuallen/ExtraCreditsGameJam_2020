using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
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

    public string villagerName;

    public int age;
    public bool gender;
    public bool gay;

    public TownResource carriedResource;
    public int baseCarryCapacity;

    public MapManager map;
    public TaskManager manager;

    public bool isWorking;
    public bool isHauling;
    public JobWorkTask workTask;
    public JobHaulTask haulTask;

    private NavPath path;
    private Vector3 pathTo;
    private Vector3 pathFrom;
    private float pathAlpha;

    Villager()
    {
    }

    void Start()
    {
        manager = GameObject.Find("MapManager").GetComponent<TaskManager>();

        baseCarryCapacity = 100;
        path = new NavPath();
        age = 21;
        gay = false;
        pathAlpha = 0;
        isWorking = false;
        currentJob = VillagerJob.IDLE;

        haulTask.finished = true;

        gender = 0.5f > Random.value;
        pathTo = transform.position;
        pathFrom = transform.position;
        position = map.toGridSpace(pathTo);

        // setPath(position, position + new Vector2Int(0, -4));

        InvokeRepeating("UpdateVillager", 0.0f, 0.5f);
    }

    void FixedUpdate()
    {
        if (pathAlpha > 0.95f)
        {
            if (path.path.Count == 0 || path.positionInPath == path.path.Count - 1)
            {
                //No more path
                position = map.toGridSpace(pathTo);
                pathAlpha = 1;
                // setPath(position, new Vector2Int(Mathf.FloorToInt(Random.value * 100), Mathf.FloorToInt(Random.value * 100)));
            }
            else
            {
                //There is more path
                pathAlpha = 0;
                position = map.toGridSpace(pathTo);
                map.moveVillager(map.toGridSpace(pathFrom), map.toGridSpace(pathTo), ID);
                pathFrom = pathTo;
                path.positionInPath++;
                pathTo = map.navGrid.toWorldSpace(path.path[path.positionInPath]);
                //is new coords blocked?
                if (map.navGrid.weights[path.path[path.positionInPath].x, path.path[path.positionInPath].y] == -1)
                {
                    //blocked, recalculate path.
                    setPath(position, path.path[path.path.Count - 1]);
                    pathTo = pathFrom;
                }
            }
            
        } else
        {
            //more path, move along path.
            pathAlpha += Time.deltaTime * 5;
            transform.position = Vector3.Lerp(pathFrom, pathTo, pathAlpha);
        }
    }

    public void MoveToLocation(Vector2Int destination)
    {
        setPath(position, destination);
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
        isHauling = true;
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
                if(position == workTask.building.entrance)
                {
                    workTask.progress += 0.5f;  // Note: this is how often UpdateVillager is invoked
                    if(workTask.progress >= workTask.timeToComplete)
                    {
                        // task completed
                        // note: the task never checks if this resource is available, so it may go into negatives!
                        // should be fixed later
                        workTask.building.RemoveResource(workTask.consumed, workTask.consumedAmount);
                        workTask.building.AddResource(workTask.produced, workTask.producedAmount);
                        workTask.job.inProgress = false;
                        workTask.finished = true;
                        this.isWorking = false;
                    }
                } else {
                    setPath(position, workTask.building.entrance);
                }

            }
        } else
        {
            //do hauling task
            if (!haulTask.finished)
            {
                // Debug.Log(position + ", " + haulTask.from.entrance);
                //there actually exists a task.
                if (position == haulTask.from.entrance && haulTask.stage == 0)
                {
                    //am currently at the giver
                    setPath(position, haulTask.to.entrance);
                    //take resource from building.

                    // foreach (KeyValuePair<TownResourceID, int> tr in haulTask.from.storedResources)
                    foreach (TownResourceID tr in System.Enum.GetValues(typeof(TownResourceID)))
                    {
                        if(tr == haulTask.item)
                        {
                            carriedResource = new TownResource(haulTask.item, 0);
                            //how many resources does the building have?
                            if (haulTask.from.storedResources[tr] < haulTask.amount)
                            {
                                //can take all
                                carriedResource.id = haulTask.item;
                                if(haulTask.amount > haulTask.from.storedResources[tr])
                                {
                                    carriedResource.amount = haulTask.from.storedResources[tr];
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
                                carriedResource.amount = haulTask.amount;
                                haulTask.from.RemoveResource(haulTask.item, haulTask.amount);
                            }
                        }
                    }
                    haulTask.stage = 1;
                }

                if(position == haulTask.to.entrance && haulTask.stage == 1)
                {
                    //am currently at the reciver
                    haulTask.to.AddResource(carriedResource.id, carriedResource.amount);
                    haulTask.amount -= carriedResource.amount;
                    carriedResource.amount = 0;
                    if(haulTask.amount > 0)
                    {
                        haulTask.stage = 0;
                        setPath(position, haulTask.from.entrance);
                    } else
                    {
                        //task is over. it's done.
                        manager.haulTaskFinished(this, haulTask);
                        haulTask.finished = true;
                        this.isHauling = false;
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
        public NativeArray<Vector2Int> path;
        public NativeArray<int> pathLength;
        public NavGrid.NavGridData grid;

        public void Execute()
        {
            NavGrid nav = new NavGrid(grid);
            path = nav.findPath(from, to, pathLength).toNativeArray(path);
        }
    }
    public void setPath(Vector2Int from, Vector2Int to)
    {
        getPathJob job = new getPathJob();
        job.from = from;
        job.to = to;
        job.grid = map.navGrid.toGridData();
        job.path = new NativeArray<Vector2Int>(1000, Allocator.Persistent);
        job.pathLength = new NativeArray<int>(1, Allocator.Persistent);
        JobHandle handle = job.Schedule();
        //once finished, set the path
        StartCoroutine(WaitForPathFinding(handle,job));
    }

    private IEnumerator WaitForPathFinding(JobHandle handle, getPathJob job)
    {
        yield return new WaitUntil(() => handle.IsCompleted);
        handle.Complete();
        path = new NavPath(job.path, job.pathLength);
        if (path.path.Count > 1)
        {
            pathFrom = map.navGrid.toWorldSpace(path.path[0]);
            pathTo = map.navGrid.toWorldSpace(path.path[1]);
            pathAlpha = 0;
        }
        job.path.Dispose();
        job.grid.weights.Dispose();
        job.pathLength.Dispose();

    }
}
