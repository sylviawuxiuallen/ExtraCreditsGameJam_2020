using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public enum TileBaseType
    {
        TILEBASE_GRASS,
        TILEBASE_DIRT,
        TILEBASE_PATH,
        TILEBASE_BUILDING
    }

    public struct TileObject
    {
        public TileType type;
        public GameObject obj;
    }

    public Tilemap tilemap;
    public NavGrid navGrid;

    public GameObject Resource_Tree;
    public GameObject Resource_Stone;
    public GameObject Resource_Ore;

    public List<Building> buildings;

    private Dictionary<TileBaseType,TileBase> tiles;
    public List<TileBaseType> __TILES_KEY;
    public List<TileBase> __TILES_VALUE;

    private Dictionary<TileBaseType, int> tileWeight;
    public List<TileBaseType> __TILEWEIGHT_KEY;
    public List<int> __TILEWEIGHT_VALUE;

    private Dictionary<Building.BuildingType, GameObject> buildingPrefabs;
    public List<Building.BuildingType> __BUILDINGPREFABS_KEY;
    public List<GameObject> __BUILDINGPREFABS_VALUE;

    private List<TileObject>[,] tileObjects;

    static int mapWidth = 200;
    static int mapHeight = 200;
    // Start is called before the first frame update
    void Start()
    {
        //Initalize Dictionaries
        tiles = new Dictionary<TileBaseType, TileBase>();
        for (int i = 0; i < __TILES_KEY.Count; i++){
            tiles[__TILES_KEY[i]] = __TILES_VALUE[i];}

        tileWeight = new Dictionary<TileBaseType, int>();
        for (int i = 0; i < __TILEWEIGHT_KEY.Count; i++){
            tileWeight[__TILEWEIGHT_KEY[i]] = __TILEWEIGHT_VALUE[i];}

        buildingPrefabs = new Dictionary<Building.BuildingType, GameObject>();
        for (int i = 0; i < __BUILDINGPREFABS_KEY.Count; i++){
            buildingPrefabs[__BUILDINGPREFABS_KEY[i]] = __BUILDINGPREFABS_VALUE[i];}

        buildings = new List<Building>();
        navGrid = new NavGrid();
        navGrid.generateGrid(mapWidth, mapHeight);
        tileObjects = new List<TileObject>[mapWidth, mapHeight];

        Debug.Log("Generating map....");
        //fill area with grass.
        for (int i = 0 - mapWidth /2; i < mapWidth/2; i++)
        {
            for(int j = 0 - mapHeight/2; j < mapHeight/2; j++)
            {
                Vector2Int gridSpace = navGrid.toGridSpace(new Vector3(i, j, 0));
                tilemap.SetTile(new Vector3Int(i, j, 0), tiles[TileBaseType.TILEBASE_GRASS]);
                tileObjects[gridSpace.x,gridSpace.y] = new List<TileObject>();
                //forrests
                //use a perlin noise
                float perlin =
                    Mathf.PerlinNoise(gridSpace.x * 0.025f, gridSpace.y * 0.025f) * 0.5f +
                    Mathf.PerlinNoise(gridSpace.x * 0.075f, gridSpace.y * 0.075f) * 0.5f;
                if(perlin >= 0.45f)
                {
                    //spawn a tree

                    tilemap.SetTile(new Vector3Int(i, j, 0), tiles[TileBaseType.TILEBASE_DIRT]);
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

        if (Input.GetMouseButtonDown(0))
        {
            PlaceBuilding(toGridSpace(Camera.main.ScreenToWorldPoint(Input.mousePosition)), buildingPrefabs[Building.BuildingType.TYPE_COTTAGE]);
        }
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
                int tileAtPos = tilemap.GetTile(tilemap.WorldToCell(navGrid.toWorldSpace(pos,transform.position))).GetHashCode();
                
                foreach (KeyValuePair<TileBaseType, TileBase> n in tiles)
                {
                    if(n.Value.GetHashCode() == tileAtPos)
                    {
                        navGrid.setWeight(pos, tileWeight[n.Key]);
                    }
                }
            }
        }
    }

    public bool PlaceBuilding(Vector2 startCoords, GameObject bdg)
    {
        int x = (int)Math.Round(startCoords.x);
        int y = (int)Math.Round(startCoords.y);

        Building newBuildingScript = bdg.GetComponent<Building>();

        int buildingWidth = newBuildingScript.width;
        int buildingHeight = newBuildingScript.height;

        Vector2Int minBounds = new Vector2Int(x, y);
        Vector2Int maxBounds = new Vector2Int(x + buildingWidth, y + buildingHeight);

        if (x + buildingWidth >= mapWidth || y + buildingHeight >= mapHeight ||
            x < 0 || y < 0)
        {
            Debug.Log("Could not place building - not enough room on map!");
            return false;
        }

        if (!CheckBuildable(minBounds, maxBounds, newBuildingScript.NaturalSiteType))
        {
            Debug.Log("Could not place building - area blocked");
            return false;
        }

        GameObject newBuilding = Instantiate(bdg, navGrid.toWorldSpace(minBounds), Quaternion.identity);

        TileObject obj = new TileObject();
        obj.type = TileType.TILE_BUILDING;
        obj.obj = newBuilding;

        Building bd = newBuilding.GetComponent<Building>();
        bd.position = minBounds;
        bd.entrance = bd.entrance + minBounds;
        buildings.Add(newBuilding.GetComponent<Building>());

        for (int _x = minBounds.x; _x < maxBounds.x; _x++)
        {
            for (int _y = minBounds.y; _y < maxBounds.y; _y++)
            {
                tileObjects[_x, _y].Add(obj);
            }
        }
        return true;
    }

    private bool CheckBuildable(Vector2Int min, Vector2Int max, NaturalSite.NaturalSiteType type)
    {
        bool hasSite = false;
        if (type == NaturalSite.NaturalSiteType.TYPE_NONE) hasSite = true;
        for(int x = min.x; x < max.x; x++)
        {
            for(int y = min.y; y< max.y; y++)
            {
                if( tileObjects[x,y].Count == 1)
                {
                    if(tileObjects[x,y][0].type == TileType.TILE_RESOURCE)
                    {
                        NaturalSite ns = tileObjects[x, y][0].obj.GetComponent<NaturalSite>();
                        
                        if (ns.siteName != type)
                        {
                            return false;
                        } else
                        {
                            hasSite = true;
                        }
                    }
                }
                if (tileObjects[x, y].Count > 1) return false;
            }
        }
        return hasSite;
    }

    public void moveVillager(Vector2Int from, Vector2Int to, int Id)
    {
        if (tileObjects[from.x, from.y].Count != 0 &&
            from.x > 0 && from.y > 0 && from.x < mapWidth && from.y < mapHeight &&
            to.x > 0 && to.y > 0 && to.x < mapWidth && to.y < mapHeight)
        {
            //find villager.
            foreach(TileObject tobj in tileObjects[from.x, from.y])
            {
                if(tobj.type == TileType.TILE_VILLAGER)
                {
                    if(tobj.obj.GetComponent<Villager>().ID == Id)
                    {
                        //this is our villager.
                        tileObjects[from.x, from.y].Remove(tobj);
                        tileObjects[to.x, to.x].Add(tobj);
                    }
                }
            }
        }
    }
}
