using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTracker : MonoBehaviour
{
    public MapManager mapManager;

    public GameObject resourceCountText;

    int[] resourceCounts;

    private void Start()
    {
        UpdateResourceCounts();
        resourceCountText.GetComponent<Text>().text = PrintResourceCounts();
    }

    public int[] UpdateResourceCounts()
    {
        TownResourceID[] resourceTypes = (TownResourceID[])System.Enum.GetValues(typeof(TownResourceID));

        resourceCounts = new int[resourceTypes.Length];

        for(int i = 0; i < resourceTypes.Length; i++)
        {
            foreach(GameObject b in mapManager.buildings)
            {
                Building s = b.GetComponent<Building>();
                foreach(TownResource r in s.storedResources)
                {
                    resourceCounts[(int)r.id] += r.amount;
                }
            }
        }

        return resourceCounts;
    }

    public string PrintResourceCounts()
    {
        TownResourceID[] resourceTypes = (TownResourceID[])System.Enum.GetValues(typeof(TownResourceID));

        string[] countStrings = new string[resourceCounts.Length];

        int i = 0;

        foreach(int c in resourceCounts)
        {
            countStrings[i] = resourceTypes[i] + ": " + c + "   ";

            i++;
        }

        string result = "";

        foreach(string s in countStrings)
        {
            Debug.Log(s);   
            result = result + s;
        }

        return result;
    }

}
