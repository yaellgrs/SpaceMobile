using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
public enum SpaceShipType { Main };

[Serializable]
public class SpaceShipDico //pour pouvoir serialiser le dictionnaire
{
    public SpaceShipType type;
    public SpaceShipData data;
}

[System.Serializable]
public class SpaceShipData
{
    public enum SpaceShipElement { Wood, Iron, Magnetic, Fire, Poison, Plasma };
    public SpaceShipElement type;

    public event Action OnTypeChanged;

    public int level = 1;
    public int fragmentlevel = 2;
    public int stage = 1;
    public bool isDead = false;
    public long lastFragmentFight = 0;

    public List<machineData> dataMachinesIron = new List<machineData>();
    public List<machineData> dataMachinesUranium = new List<machineData>();
    [JsonIgnore] public List<machineIronElement> machinesIron = new List<machineIronElement>();
    [JsonIgnore] public List<machineUraniumElement> machinesUranium = new List<machineUraniumElement>();

    public Dictionary<UpgradesIronElement.UpgradeType, UpgradeData> dataUpgradesIron = new Dictionary<UpgradesIronElement.UpgradeType, UpgradeData>();
    public Dictionary<UpgradesUraniumElement.UpgradeType, UpgradeData> dataUpgradesUranium = new Dictionary<UpgradesUraniumElement.UpgradeType, UpgradeData>();
    [JsonIgnore] public List<UpgradesElement> upgradesIron = new List<UpgradesElement>();
    [JsonIgnore] public List<UpgradesElement> upgradesUranium = new List<UpgradesElement>();

    public Dictionary<UpgradesShipElement.UpgradeType, UpgradeData> dataUpgradesShip = new Dictionary<UpgradesShipElement.UpgradeType, UpgradeData>();
    [JsonIgnore] public List<UpgradesElement> upgradesShip = new List<UpgradesElement>();

    [JsonIgnore] public ShipTempStat damage;
    [JsonIgnore] public ShipTempStat lifeMax;
    [JsonIgnore] public ShipTempStat shieldMax;
    public BigNumber life = new BigNumber(0);
    public BigNumber shield = new BigNumber(0);
    public BigNumber regenShield = new BigNumber(0);

    public BigNumber iron = new BigNumber(0);
    public BigNumber uranium = new BigNumber(0);

    public BigNumber BN_xp { get; private set; } = new BigNumber(0);
    public BigNumber BN_xpMax = new BigNumber(100);

    public SpaceShipData()
    {

    }

    #region ------ load ----------
    public void Load(bool reset = false)
    {

        LoadMachines(reset);
        LoadUpgrades(reset);

        InitTempData();
        MainUi.Instance?.xpUI?.loadBonus();
        Stats.Instance.AddIron(new BigNumber(0));
        Stats.Instance.AddUranium(new BigNumber(0));
        
        if(life.isBigger(lifeMax.getTotal()))
            life.Set(lifeMax.getTotal());
        if (shield.isBigger(shieldMax.getTotal()))
            shield.Set(shieldMax.getTotal());
    }

    private void LoadMachines(bool reset = false)
    {
        if (dataMachinesIron.Count == 0 || reset)
        {
            dataMachinesIron = new List<machineData>
            {
                new machineData("Anvil", new BigNumber(10)),
                new machineData("ironMachine", new BigNumber(1, 3)),
                new machineData("ironMachines", new BigNumber(1, 6)),
                new machineData("usine", new BigNumber(1, 9)),
                new machineData("usines", new BigNumber(1, 12))
            };
        }
        if (dataMachinesUranium.Count == 0 || reset)
        {
            dataMachinesUranium = new List<machineData>
            {
                new machineData("Anvil", new BigNumber(10)),
                new machineData("ironMachine", new BigNumber(5, 3)),
                new machineData("ironMachines", new BigNumber(5, 6)),
                new machineData("usine", new BigNumber(5, 9)),
                new machineData("usines", new BigNumber(5, 12))
            };
        }

        machinesIron.Clear();
        foreach (var data in dataMachinesIron){
            machineIronElement m = new machineIronElement(data);
            if (!machinesIron.Contains(m))
                machinesIron.Add(m);
        }

        machinesUranium.Clear();
        foreach (var data in dataMachinesUranium){
            machineUraniumElement m = new machineUraniumElement(data);
            if (!machinesUranium.Contains(m))
                machinesUranium.Add(m);
        }
    }

    private void LoadUpgrades(bool reset = false)
    {
        if (dataUpgradesIron.Count == 0 || reset)
        {
            dataUpgradesIron.Clear();
            foreach (UpgradesIronElement.UpgradeType type in Enum.GetValues(typeof(UpgradesIronElement.UpgradeType)))
            {
                dataUpgradesIron[type] = new UpgradeData();
            }
        }
        if (dataUpgradesUranium.Count == 0 || reset)
        {
            dataUpgradesUranium.Clear();
            foreach (UpgradesUraniumElement.UpgradeType type in Enum.GetValues(typeof(UpgradesUraniumElement.UpgradeType)))
            {
                dataUpgradesUranium[type] = new UpgradeData();
            }
        }
        if (dataUpgradesShip.Count == 0 || reset)
        {
            dataUpgradesShip.Clear();
            foreach (UpgradesShipElement.UpgradeType type in Enum.GetValues(typeof(UpgradesShipElement.UpgradeType)))
            {
                dataUpgradesShip[type] = new UpgradeData();
            }
        }


        upgradesIron.Clear();
        upgradesUranium.Clear();
        upgradesShip.Clear();
        foreach (var data in dataUpgradesIron)
        {
            UpgradesIronElement up = new UpgradesIronElement(data.Value, data.Key.ToString(), data.Key);
            if (!upgradesIron.Contains(up))
                upgradesIron.Add(up);
        }
        foreach (var data in dataUpgradesUranium)
        {
            UpgradesUraniumElement up = new UpgradesUraniumElement(data.Value, data.Key.ToString(), data.Key);
            if (!upgradesUranium.Contains(up))
                upgradesUranium.Add(up);
        }
        foreach (var data in dataUpgradesShip)
        {
            UpgradesShipElement up = new UpgradesShipElement(data.Value, data.Key.ToString(), data.Key);
            if (!upgradesShip.Contains(up))
                upgradesShip.Add(up);
        }
    }

    public void InitTempData()
    {
        damage = new ShipTempStat();
        lifeMax = new ShipTempStat();
        shieldMax = new ShipTempStat();

        LoadBonus();
    }

    public void LoadBonus()
    {
        foreach(var upgrade in upgradesIron)
            upgrade.SetReward();
        foreach (var upgrade in upgradesUranium)
            upgrade.SetReward();
        foreach (var upgrade in Stats.Instance.upgradesPrestige)
            upgrade.SetReward();
        foreach (var up in upgradesShip)
            up.SetReward();
    }


    #endregion

    #region ------ add ----------
    public void AddXP(BigNumber amount)
    {
        BN_xp += amount;
        if (BN_xp > BN_xpMax)
        {
            MainUi.Instance.xpUI.LevelUp();
        }
        MainUi.Instance.upLevelUI();
    }
    public void SetNextType(int amount = 1)
    {
        type = (SpaceShipData.SpaceShipElement)Unity.Mathematics.math.clamp((int)Ship.Current.type + amount, 0, System.Enum.GetValues(typeof(SpaceShipData.SpaceShipElement)).Length - 1);
        
        level = 1;
        fragmentlevel = 2;
        stage = 1;
        isDead = false;

        life = new BigNumber(0);
        shield = new BigNumber(0);

        iron = new BigNumber(0);
        uranium = new BigNumber(0);

        BN_xp = new BigNumber(0);

        Load(true);

        OnTypeChanged?.Invoke();

    }

    #endregion

    public bool HaveUranium()
    {
        return type != SpaceShipData.SpaceShipElement.Wood;
    }

}

public class ShipTempStat
{
    //stocker ailleurs qu'ici ou stats pour ne pas sauvegarder cette donné, c'est inutile de la sauvegarder
    public BigNumber initial = new BigNumber(5);
    public float prestige_multiplicator = 1f;
    public float rocket_multiplicator = 1f;
    public int critical_multiplicator = 5;

    public BigNumber getTotal()
    {

        float leveBonus = 1f + (Ship.Current.level - 1) * 0.1f;
        BigNumber total = new BigNumber(initial);
        total *= prestige_multiplicator * leveBonus;

        return total;
    }
    public BigNumber getTotal(bool isRocket, bool critical)
    {
        BigNumber damage = getTotal();
        if (isRocket) damage *= rocket_multiplicator;
        if (critical) damage *= critical_multiplicator;
        return damage;
    }
}