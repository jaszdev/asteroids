using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{
    public static List<Transform> ColliderListToListTransform(List<Collider2D> list) 
    {
        var count = list.Count;
        var transformList = new List<Transform>(count);
        for (var i = 0; i < count; i++) transformList.Insert(i, list[i].transform);
        return transformList;
    }
 }
