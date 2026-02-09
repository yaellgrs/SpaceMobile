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

    public void LoadShips()
    {
        Dictionary<SpaceShipType, SpaceShipData> ships = new Dictionary<SpaceShipType, SpaceShipData>();
        ships[SpaceShipType.Basic] = new SpaceShipData();
        ships[SpaceShipType.Fire] = new SpaceShipData();
        ships[SpaceShipType.Ice] = new SpaceShipData();

        if (Stats.Instance == null) return;
        foreach (var (key, value) in ships)
        {
            if (Stats.Instance.spaceShips.Find(e => e.type == key) == null)
            {
                Stats.Instance.spaceShips.Add(new SpaceShipDico
                {
                    type = key,
                    data = value
                });
            }
        }
        SwitchShip(Stats.Instance.currentSpaceShipType);
    }

    public void SwitchShip(SpaceShipType type)
    {
        Stats.Instance.currentSpaceShipType = type;
        Ship.Current.Load();

        spaceShip.instance.LoadAnimation();

        Debug.Log("life : " + Ship.Current.life + "lifeMax : " + Ship.Current.lifeMax.getTotal());
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
