using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This stores naturally occuring map features, such as trees, ore veins, etc.

public abstract class NaturalSite : MonoBehaviour
{
    public enum NaturalSiteType
    {
        TYPE_NONE,
        TYPE_STONE,
        TYPE_ORE,
        TYPE_TREE
    }
    public NaturalSiteType siteName;
}
