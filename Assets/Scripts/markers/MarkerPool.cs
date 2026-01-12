using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
public class MarkerPool : MonoBehaviour
{
    private List<XpMarker> pool = new List<XpMarker>();

    public MarkerType type;
    public XpMarker prefab;

    public MarkerPool(XpMarker NewPrefab, MarkerType newType)
    {
        prefab = NewPrefab;
        type = newType;

        XpMarker obj = Instantiate(prefab);
        obj.gameObject.SetActive(false);
        pool.Add(obj);
    }

    public XpMarker GetPrefab()
    {
        if (pool.Count > 0)
        {
            XpMarker obj = pool[0];
            obj.gameObject.SetActive(true);
            pool.RemoveAt(0);
            return obj;
        }
        else
        {
            XpMarker obj = Instantiate(prefab);
            return obj;
        }
    }

    public void returnPrefab(XpMarker marker)
    {
        marker.gameObject.SetActive(false);
        pool.Add(marker);
            
    }
}
