using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class NavPath
{
    public int positionInPath = 0;
    public List<Vector2Int> path;
    public NavPath()
    {
        path = new List<Vector2Int>();
    }

    public NavPath(NativeArray<Vector2Int> arr, NativeArray<int> pathLength)
    {
        path = new List<Vector2Int>();
        for(int i = 0; i < pathLength[0]; i++)
        {
            path.Add(arr[i]);
        }
    }
    public void drawPath(Color col, NavGrid grid)
    {
        for(int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(
                grid.toWorldSpace(path[i],new Vector3(0.5f,0.5f,0)),
                grid.toWorldSpace(path[i+1], new Vector3(0.5f, 0.5f, 0)),
                col, 50);
        }
    }

    public NativeArray<Vector2Int> toNativeArray(NativeArray<Vector2Int> arr)
    {
        for (int i = 0; i < path.Count; i++)
            arr[i] = path[i];
        return arr;
    }

}
