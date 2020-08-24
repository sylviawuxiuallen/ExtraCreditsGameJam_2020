using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTracker : MonoBehaviour
{
    public MapManager mapManager;

    public Dictionary<TownResourceID, int> resourceCounts;             // The actual amount of resources in storage
    public Dictionary<TownResourceID, int> reservedResourceCounts;     // The amount of resources that are reserved for pending/current jobs

    private Dictionary<TownResourceID, GameObject> resourceCountTextObjects;
    public List<TownResourceID> __RESOURCECOUNTTEXTOBJECTS_KEY;
    public List<GameObject> __RESOURCECOUNTTEXTOBJECTS_VALUE;

    private void Start()
    {
        resourceCounts = new Dictionary<TownResourceID, int>();
        reservedResourceCounts = new Dictionary<TownResourceID, int>();
        foreach (TownResourceID r in System.Enum.GetValues(typeof(TownResourceID)))
        {
            resourceCounts.Add(r, 0);
            reservedResourceCounts.Add(r, 0);
        }
        resourceCountTextObjects = new Dictionary<TownResourceID, GameObject>();
        for (int i = 0; i < __RESOURCECOUNTTEXTOBJECTS_KEY.Count; i++) { 
            resourceCountTextObjects[__RESOURCECOUNTTEXTOBJECTS_KEY[i]] = __RESOURCECOUNTTEXTOBJECTS_VALUE[i]; }
        UpdateResourceCounts();
        UpdateUI();
    }

    private void Update()
    {
        UpdateResourceCounts();
        UpdateUI();
    }

    public bool TryReserveResources(Dictionary<TownResourceID, int> spendAmounts)
    {
        foreach(KeyValuePair<TownResourceID, int> s in spendAmounts)
        {
            if(s.Value > AvailableResourceCounts()[s.Key])
            {
                return false;
            }
        }

        foreach (KeyValuePair<TownResourceID, int> s in spendAmounts)
        {
            AvailableResourceCounts()[s.Key] -= s.Value;
            reservedResourceCounts[s.Key] += s.Value;
        }

        UpdateUI();
        return true;
    }

    public void UpdateResourceCounts()
    {
        foreach (TownResourceID r in System.Enum.GetValues(typeof(TownResourceID)))
        {
            resourceCounts[r] = 0;
            reservedResourceCounts[r] = 0;
        }

        foreach (Building b in mapManager.buildings)
        {
            if (b.storedResources != null)
            {
                foreach (KeyValuePair<TownResourceID, int> r in b.storedResources)
                {
                    resourceCounts[r.Key] += r.Value;
                }
                foreach (KeyValuePair<TownResourceID, int> r in b.reservedResources)
                {
                    reservedResourceCounts[r.Key] += r.Value;
                }
            }
        }
    }

    public Dictionary<TownResourceID, int> AvailableResourceCounts()
    {
        Dictionary<TownResourceID, int> result = new Dictionary<TownResourceID, int>();

        foreach (TownResourceID r in System.Enum.GetValues(typeof(TownResourceID)))
        {
            result.Add(r, resourceCounts[r] - reservedResourceCounts[r]);
        }

        return result;
    }

    public void UpdateUI()
    {
        foreach (KeyValuePair<TownResourceID, GameObject> pair in resourceCountTextObjects)
        {
            pair.Value.GetComponent<Text>().text = AvailableResourceCounts()[pair.Key].ToString();
        }
    }

    public Building FindResourceStock(KeyValuePair<TownResourceID, int> r)
    {
        foreach(Building b in mapManager.buildings)
        {
            if(b.storedResources[r.Key] >= r.Value) {
                return b;
            }
        }

        return null;
    }
}
