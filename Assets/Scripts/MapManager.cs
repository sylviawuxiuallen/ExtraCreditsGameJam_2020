﻿using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public enum TileType
    {
        TILE_VILLAGER,
        TILE_BUILDING,
        TILE_RESOURCE
    }

    public struct TileObject
    {
        public TileType type;
        public GameObject obj;
    }

    static int mapWidth = 200;
    static int mapHeight = 200;


    public Tilemap tilemap;
    public TileBase[] tiles;
    private int[] tileWeight = {0,5,10,-1 };

    public NavGrid navGrid;
    private List<TileObject>[,] tileObjects;
    //Resource objects

    public GameObject Resource_Tree;
    public GameObject Resource_Stone;
    public GameObject Resource_Ore;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Generating map....");

        navGrid = new NavGrid();
        navGrid.generateGrid(mapWidth, mapHeight);
        tileObjects = new List<TileObject>[mapWidth, mapHeight];

        //fill area with grass.
        for (int i = 0 - mapWidth /2; i < mapWidth/2; i++)
        {
            for(int j = 0 - mapHeight/2; j < mapHeight/2; j++)
            {
                Vector2Int gridSpace = navGrid.toGridSpace(new Vector3(i, j, 0));
                tilemap.SetTile(new Vector3Int(i, j, 0), tiles[1]);
                tileObjects[gridSpace.x,gridSpace.y] = new List<TileObject>();
                //forrests
                //use a perlin noise
                float perlin =
                    Mathf.PerlinNoise(gridSpace.x * 0.025f, gridSpace.y * 0.025f) * 0.5f +
                    Mathf.PerlinNoise(gridSpace.x * 0.075f, gridSpace.y * 0.075f) * 0.5f;
                if(perlin >= 0.45f)
                {
                    //spawn a tree

                    tilemap.SetTile(new Vector3Int(i, j, 0), tiles[2]);
                    TileObject obj = new TileObject();
                    obj.type = TileType.TILE_RESOURCE;
                    obj.obj = Instantiate(Resource_Tree, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
                    tileObjects[gridSpace.x, gridSpace.y].Add(obj);
                }
            }
        }
        
        Debug.Log("Updating Weights");
        setNavGridWeightsFromTileMap();
        //navGrid.drawGrid();
        
    }

    void Update()
    {
        /* Pathfinding tester code
        float T1 = Time.realtimeSinceStartup;
        NavPath path = navGrid.findPath(new Vector2Int(0, 0), new Vector2Int(mapWidth - 1, mapHeight - 1));
        path.drawPath(Color.red);
        float T2 = Time.realtimeSinceStartup;
        Debug.LogFormat("Time elapsed {0}ms",(T2-T1) * 1000);*/
    }

    public Vector2Int toGridSpace(Vector3 pos)
    {
        float resolution = 1.0f;

        //grid is center oriented.
        //width/2,height/2 is current position.

        Vector2 _pos = pos - transform.position;

        return new Vector2Int(Mathf.FloorToInt(_pos.x / resolution + mapWidth * 0.5f), Mathf.FloorToInt(_pos.y / resolution + mapHeight * 0.5f));
    }

    /// <summary>
    /// Updates the weights of the navmesh from the tile map.
    /// EXPENSIVE, update navGrid manually where possible!
    /// </summary>
    public void setNavGridWeightsFromTileMap()
    {
        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                TileBase tileAtPos = tilemap.GetTile(tilemap.WorldToCell(navGrid.toWorldSpace(pos,transform.position)));
                int tileMatch = -1;
                for(int i = 0; i < tiles.Length; i++)
                {
                    if(tileAtPos.GetHashCode() == tiles[i].GetHashCode())     //string compare. UGLY!
                    {
                        tileMatch = i;
                    }
                }
                navGrid.setWeight(pos, tileWeight[tileMatch]);
            }
        }
    }
}