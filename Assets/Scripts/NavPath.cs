using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPath
{
    public int positionInPath = 0;
    public List<Vector2Int> path;
    public NavGrid grid;
    public NavPath()
    {
        path = new List<Vector2Int>();
    }
    public void drawPath(Color col)
    {
        for(int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(
                grid.toWorldSpace(path[i],new Vector3(0.5f,0.5f,0)),
                grid.toWorldSpace(path[i+1], new Vector3(0.5f, 0.5f, 0)),
                col, 50);
        }
    }
}
