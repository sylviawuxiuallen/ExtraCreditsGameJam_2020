using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTracker : MonoBehaviour
{
    public MapManager mapManager;

    Dictionary<TownResourceID, int> resourceCounts;

    private Dictionary<TownResourceID, GameObject> resourceCountTextObjects;
    public List<TownResourceID> __RESOURCECOUNTTEXTOBJECTS_KEY;
    public List<GameObject> __RESOURCECOUNTTEXTOBJECTS_VALUE;

    private void Start()
    {
        resourceCounts = new Dictionary<TownResourceID, int>();
        resourceCountTextObjects = new Dictionary<TownResourceID, GameObject>();
        for (int i = 0; i < __RESOURCECOUNTTEXTOBJECTS_KEY.Count; i++) { 
            resourceCountTextObjects[__RESOURCECOUNTTEXTOBJECTS_KEY[i]] = __RESOURCECOUNTTEXTOBJECTS_VALUE[i]; }
        UpdateResourceCounts();
        UpdateUI();
    }

    public void UpdateResourceCounts()
    {
        foreach (TownResourceID r in System.Enum.GetValues(typeof(TownResourceID)))
        {
            resourceCounts[r] = 0;
        }

        foreach (Building b in mapManager.buildings)
        {
            foreach(TownResource r in b.storedResources)
            {
                resourceCounts[r.id] += r.amount;
            }
        }
    }

    private void UpdateUI()
    {
        foreach (KeyValuePair<TownResourceID, GameObject> pair in resourceCountTextObjects)
        {
            pair.Value.GetComponent<Text>().text = resourceCounts[pair.Key].ToString();
        }
    }
}
