using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance;
    private void Awake()
    {
        if (Instance == null) { 
            Instance = this;
            LoadShips();
        }
        else Destroy(gameObject);
    }

    private void LoadShips()
    {
        Dictionary<SpaceShipType, SpaceShipData> ships = new Dictionary<SpaceShipType, SpaceShipData>();
        ships[SpaceShipType.Basic] = new SpaceShipData();
        ships[SpaceShipType.Fire] = new SpaceShipData();
        ships[SpaceShipType.Ice] = new SpaceShipData();

        if (Stats.Instance == null) return;
/*        foreach(var (key, value) in ships)
            if (Stats.Instance.spaceShips.ContainsKey(key)) Stats.Instance.spaceShips[key] = value;*/
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
