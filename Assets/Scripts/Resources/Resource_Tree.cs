using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Tree : NaturalSite
{
    public List<Sprite> treeSprites;
    void Start()
    {
        siteName = NaturalSiteType.TYPE_TREE;
        //vary size and position a little
        Vector3 pos = (Random.insideUnitSphere * 0.25f);
        pos.z = transform.position.y / 200;
        transform.position += pos;
        transform.localScale = transform.localScale * (Random.value + 1);
        gameObject.GetComponent<SpriteRenderer>().sprite = treeSprites[Mathf.FloorToInt(Random.value * treeSprites.Count)];
    }
}
