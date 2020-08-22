using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    enum mapObjectType
    {
        TYPE_VILLAGER,
        TYPE_BUILDING,
        TYPE_RESOURCE
    }
    struct mapObject
    {
        mapObjectType objectType;
        GameObject gameObject;
    }
    NavGrid navigationGrid;
    List<mapObject>[,] SceneObjects; //grid of lists. God preserve us.

    public bool spawnBuilding(Vector2Int pos /*building base class*/)
    {
        return false;
    }

    public bool spawnVillager(Vector2Int pos /*villager base class*/)
    {
        return false;
    }

    public void removeBuilding(Vector2Int pos,int ind /*building base class*/)
    {

    } 

    public void removeVillager(Vector2Int pos /*villager base class*/)
    {

    }

    public void moveVillager(Vector2Int from, Vector2Int to)
    {

    }

    public List<GameObject> findBuildings(string buildingType)
    {
        return new List<GameObject>();
    }
}
