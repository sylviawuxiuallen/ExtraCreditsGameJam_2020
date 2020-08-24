using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class NavGrid
{

    public struct NavGridData
    {
        public NativeArray<int> weights;
        public float resolution;
        public int width;
        public int height;
    }

    public int[,] weights;
    public float resolution = 1f;
    public int width;
    public int height;

    public NavGrid() { }

    public NavGrid(NavGridData d)
    {
        width = d.width;
        height = d.height;
        weights = new int[width, height];
        for(int i = 0; i < width; i++)
            for(int j = 0; j < height; j++)
                weights[i, j] = d.weights[j + i * height];

        resolution = d.resolution;
    }

    public NavGridData toGridData()
    {
        NavGridData data = new NavGridData();
        data.height = height;
        data.width = width;
        data.resolution = resolution;

        data.weights = new NativeArray<int>(width * height, Allocator.Persistent);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                data.weights[j + i*height] = weights[i, j];
            }
        }
        return data;
    }

    /// <summary>
    /// Converts a world position to grid space.
    /// </summary>
    /// <param name="pos"> Position in world space.</param>
    /// /// <param name="objectOrigin"> origin of parent object </param>
    /// <returns> Position in grid space.</returns>
    public Vector2Int toGridSpace(Vector3 pos, Vector3 objectOrigin)
    {
        //grid is center oriented.
        //width/2,height/2 is current position.

        Vector2 _pos = pos - objectOrigin;

        return new Vector2Int(Mathf.FloorToInt(_pos.x / resolution + width / 2), Mathf.FloorToInt(_pos.y / resolution + height / 2));
    }

    public Vector2Int toGridSpace(Vector3 pos)
    {
        //grid is center oriented.
        //width/2,height/2 is current position.

        Vector2 _pos = pos;

        return new Vector2Int(Mathf.FloorToInt(_pos.x / resolution + width / 2), Mathf.FloorToInt(_pos.y / resolution + height / 2));
    }

    /// <summary>
    /// Converts a grid position to world space.
    /// </summary>
    /// <param name="pos"> Position in grid/</param>
    /// <param name="objectOrigin"> origin of parent object </param>
    /// <returns> Position in world space.</returns>
    public Vector3 toWorldSpace(Vector2Int pos, Vector3 objectOrigin)
    {
        return objectOrigin + new Vector3((pos.x - width / 2) * resolution, (pos.y - height / 2) * resolution, 0);
    }

    public Vector3 toWorldSpace(Vector2Int pos)
    {
        return new Vector3((pos.x - width / 2) * resolution, (pos.y - height / 2) * resolution, 0);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///// Pathing //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns a path from the start to the end.
    /// </summary>
    /// <param name="start">Start of the path.</param>
    /// <param name="end">End of the path.</param>
    /// <returns>Path to the end from start.</returns>
    public NavPath findPath(Vector2Int start, Vector2Int end, NativeArray<int> pathLength)
    {
        NavPath path = new NavPath();
        //uses A* alg.
        //head towards point, if run into wall, then branch out. 
        char[,] checkedLocations = new char[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                checkedLocations[x, y] = (char) 0;
            }
        }
        Vector2Int pos = start;
        int backtracks = 0;
        int loops = 0;
        while(pos.x != end.x || pos.y != end.y)
        {
            loops++;
            //Debug.Log(pos);
            checkedLocations[pos.x, pos.y] = (char) 1;
            Vector2Int[] directions =
            {
                pos + new Vector2Int(1,0),
                pos - new Vector2Int(1,0),
                pos + new Vector2Int(0,1),
                pos - new Vector2Int(0,1)
            };
            //Debug.LogFormat("Directions:\n{0} | {1} | {2} | {3}", directions[0], directions[1], directions[2], directions[3]);
            float[] directionsWeight = { 
                aStar(directions[0],end,checkedLocations),
                aStar(directions[1],end,checkedLocations),
                aStar(directions[2],end,checkedLocations),
                aStar(directions[3],end,checkedLocations)
            };
            
            //Debug.LogFormat("Dweight:\n{0} | {1} | {2} | {3}", directionsWeight[0], directionsWeight[1], directionsWeight[2], directionsWeight[3]);
            //try direction with least value.
            float minVal = float.MaxValue;
            int minValInd = 0;
            for(int i = 0; i < 4; i++)
            {
                if(minVal > directionsWeight[i])
                {
                    minVal = directionsWeight[i];
                    minValInd = i;
                }
            }
            //Debug.LogFormat("MinVal : {0}\nMinInd : {1}",minVal,minValInd);
            if(minVal > int.MaxValue)
            {
                //move backwards one space.
                if(path.path.Count > 0)
                {
                    pos = path.path.Last();
                    path.path.RemoveAt(path.path.Count - 1);
                    backtracks++;
                } else
                {
                    //tried everything, no path to goal.
                    //Debug.LogFormat("Error, no path to goal\n");
                    return path;
                }
            } else
            {
                //go towards minVal
                checkedLocations[pos.x, pos.y] = (char)2;
                path.path.Add(pos);
                pos = directions[minValInd];
            }
        }
        //need to simplify path.
        //merge verticies which are next to each other but not next to each other by number.
        //worst case n*n time.
        //path.drawPath(Color.blue);
        //Debug.Log("Simplifying path");
        for(int i = 1; i < path.path.Count - 1; i++)
        {
            //only look ahead.
            Vector2Int prevPoint = path.path[i - 1];
            Vector2Int nextPoint = path.path[i + 1];
            //check points around.
            if(path.path[i].x > 0 && path.path[i].y > 0 && path.path[i].x < width -1 && path.path[i].y < height -1)
            {
                Vector2Int[] directions =
                {
                    path.path[i] + new Vector2Int(1,0),
                    path.path[i] - new Vector2Int(1,0),
                    path.path[i] + new Vector2Int(0,1),
                    path.path[i] - new Vector2Int(0,1)
                };
                Vector2Int MergeablePath = new Vector2Int(-1, -1);
                foreach (Vector2Int x in directions)
                {
                    if(checkedLocations[x.x, x.y] == 2 && x != prevPoint && x != nextPoint)
                    {
                        MergeablePath = x;
                    }
                }
                if(MergeablePath.x != -1)
                {
                    //there is a mergeable path
                    //Debug.LogFormat("Mergeable path at: [{0}]{1}->{2}",i,path.path[i], MergeablePath);
                    //go along path until you hit the mergeable path
                    for (int j = i + 1; j < path.path.Count; j++)
                    {
                        checkedLocations[path.path[j].x, path.path[j].y] = (char)1;
                        if (MergeablePath == path.path[j])
                        {
                            //found the path
                            //Debug.LogFormat("Found mergeable path");
                            if (j - i >= 2)
                            {
                                //Debug.LogFormat("Removing... {0} to {1}",  i+1, j-1);
                                path.path.RemoveRange(i + 1, j - i - 1);
                                j = path.path.Count;
                            }
                        }
                    }
                }
            }
        }
        
        path.path.Add(end); // ugly hack to fix pathing bug!

        pathLength[0] = path.path.Count;


        return path;
    }

    /// <summary>
    /// Generates a "goodness" value for a point with the goal of reaching the end point.
    /// </summary>
    /// <param name="pos">Position to evaluate</param>
    /// <param name="end">Destination</param>
    /// <returns></returns>
    private float aStar(Vector2Int pos, Vector2Int end, char[,] checkedLocations)
    {
        float distance = (pos - end).magnitude;
        if (pos.x >= width || pos.y >= height || pos.x < 0 || pos.y < 0) 
        {
            return float.MaxValue; 
        }
        if(checkedLocations[pos.x,pos.y] > 0)
        { 
            return float.MaxValue; 
        }
        float weight = weights[pos.x, pos.y];

        if(weight == -1) { return float.MaxValue; }

        return (distance) + (weight);
    }

    /// <summary>
    /// Sets the pathing weight of a grid point.
    /// </summary>
    /// <param name="pos"> Position to set. </param>
    /// <param name="weight"> Pathing weight, -1 is impassible. </param>
    public void setWeight(Vector2Int pos, int weight)
    {
        if (pos.x < width && pos.y < height && pos.x >= 0 && pos.y >= 0)
            weights[pos.x, pos.y] = weight;
    }


    /// <summary>
    /// Generates a grid
    /// </summary>
    /// <param name="width">Width of grid.</param>
    /// <param name="height">Height of grid.</param>
    /// <param name="defaultWeight">Weight of each point.</param>
    public void generateGrid(int _width, int _height, int defaultWeight)
    {
        //make new array.
        width = _width;
        height = _height;
        weights = new int[_width,height];
        //preinitalize array.
        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                weights[x, y] = defaultWeight;
            }
        }
    }

    /// <summary>
    /// Debug draws the grid.
    /// </summary>
    public void drawGrid()
    {
        //draw grid lines;
        for (int x = 0; x < width; x++)
        {
            Debug.DrawLine(toWorldSpace(new Vector2Int(x, 0)), toWorldSpace(new Vector2Int(x, height)), Color.green,100);
        }
        for (int y = 0; y < height; y++)
        {
            Debug.DrawLine(toWorldSpace(new Vector2Int(0, y)), toWorldSpace(new Vector2Int(width, y)), Color.green,100);
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.DrawLine(
                    toWorldSpace(new Vector2Int(x+1, y+1)),
                    toWorldSpace(new Vector2Int(x, y)),
                    Color.HSVToRGB((weights[x,y] / 10.0f) + 0.1f,1,1),
                    100);
            }
            
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///// Overloads /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the pathing weight of a grid point.
    /// </summary>
    /// <param name="pos"> Position to set. </param>
    /// <param name="weight"> Pathing weight, -1 is impassible. </param>
    public void setWeight(Vector3 pos, int weight)
    {
        setWeight(toGridSpace(pos), weight);
    }

    /// <summary>
    /// Generates a grid
    /// </summary>
    /// <param name="width">Width of grid.</param>
    /// <param name="height">Height of grid.</param>
    public void generateGrid(int _width, int _height)
    {
        generateGrid(_width, _height, 0);
    }
}
