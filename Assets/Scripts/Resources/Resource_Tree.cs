using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Tree : NaturalSite
{

    void Start()
    {
        siteName = NaturalSiteType.TYPE_TREE;
        //vary size and position a little
        Vector3 pos = (Random.insideUnitSphere * 0.25f);
        pos.z = 0;
        transform.position += pos;
        transform.localScale = transform.localScale * (Random.value + 1);
    }
}
