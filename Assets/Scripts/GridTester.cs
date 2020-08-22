using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTester : MonoBehaviour
{
    NavGrid myGrid;
    // Start is called before the first frame update
    void Start()
    {
        myGrid = GetComponent<NavGrid>();
        myGrid.generateGrid(10, 10, 5);
        myGrid.setWeight(new Vector2Int(9, 4), -1);
        myGrid.setWeight(new Vector2Int(8, 4), -1);
        myGrid.setWeight(new Vector2Int(7, 4), -1);
        myGrid.setWeight(new Vector2Int(6, 4), -1);
        myGrid.setWeight(new Vector2Int(5, 4), -1);
        myGrid.setWeight(new Vector2Int(5, 5), -1);
        myGrid.setWeight(new Vector2Int(5, 6), -1);
        myGrid.setWeight(new Vector2Int(5, 7), -1);
        myGrid.setWeight(new Vector2Int(5, 8), -1);
        myGrid.drawGrid();
        NavPath myPath = myGrid.findPath(new Vector2Int(0, 0), new Vector2Int(9, 5));
        myPath.drawPath(Color.red);
    }
}
