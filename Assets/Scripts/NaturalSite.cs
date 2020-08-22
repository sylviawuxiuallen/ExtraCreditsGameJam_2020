using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This stores naturally occuring map features, such as trees, ore veins, etc.

public abstract class NaturalSite : MonoBehaviour
{
    public static bool obstacle;    // Does this site block villager movement?
    public static string siteName;
    public static Sprite siteSprite;
}
