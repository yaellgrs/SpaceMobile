using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public enum MarkerType { Xp, Diamand, Damage, Iron, Uranium, Critique, Prestige };

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public XpMarker XpPrefab;
    public XpMarker DiamandPrefab;
    public XpMarker DamagePrefab;
    public XpMarker CritiquePrefab;
    public XpMarker IronPrefab;
    public XpMarker UraniumPrefab;
    public XpMarker PrestigePrefab;

    MarkerPool xp_pool;
    MarkerPool diamand_pool;
    MarkerPool damage_pool;
    MarkerPool iron_pool;
    MarkerPool uranium_pool;
    MarkerPool prestige_poll;

    private List<MarkerPool> pools = new List<MarkerPool>();



    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        xp_pool = new MarkerPool(XpPrefab, MarkerType.Xp);
        diamand_pool = new MarkerPool(DiamandPrefab, MarkerType.Diamand);
        damage_pool = new MarkerPool(DamagePrefab, MarkerType.Damage);
        iron_pool = new MarkerPool(IronPrefab, MarkerType.Iron);
        uranium_pool = new MarkerPool(UraniumPrefab, MarkerType.Uranium);
        MarkerPool critique_pool = new MarkerPool(CritiquePrefab, MarkerType.Critique);
        MarkerPool prestige_pool = new MarkerPool(PrestigePrefab, MarkerType.Prestige);

        pools.Add(xp_pool);
        pools.Add(diamand_pool);
        pools.Add(damage_pool);
        pools.Add(iron_pool);
        pools.Add(uranium_pool);
        pools.Add(critique_pool);
        pools.Add(prestige_pool);

    }

    public XpMarker GetPrefab(MarkerType type)
    {
        foreach(MarkerPool pool in pools)
        {
            if(pool.type == type)
            {
                return pool.GetPrefab();
            }
        }

        return null;
    }

    public void returnPrefab(XpMarker marker)
    {
        foreach (MarkerPool pool in pools)
        {
            if (pool.type == marker.type)
            {
                pool.returnPrefab(marker);
                return;
            }
        }
    }
    
    public void LaunchPrefab(Vector3 position, string xp, MarkerType type)
    {
        XpMarker marker = Instance.GetPrefab(type);
        marker.init(position, xp);
    }
    public void LaunchPrefab(Vector3 position, string xp, MarkerType type, float speed, float alpha_decrease)
    {
        XpMarker marker = Instance.GetPrefab(type);
        marker.init(position, xp);
        marker.speed = speed;
        marker.alpha_decrease = alpha_decrease;
    }

}
