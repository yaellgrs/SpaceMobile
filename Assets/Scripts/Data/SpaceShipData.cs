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

    public int level = 1;
    public int fragmentlevel = 2;
    public int stage = 1;
    public bool isDead = false;
    public long lastFragmentFight = 0;

    public List<machineIronElement> machineIron = new List<machineIronElement>();
    public List<machineUraniumElement> machinesUranium = new List<machineUraniumElement>();

    public List<UpgradesElement> upgradesIron = new List<UpgradesElement>();
    public List<UpgradesElement> upgradesUranium = new List<UpgradesElement>();

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
 
    public void Load()
    {
        LoadMachines();
        LoadUpgrades();

        InitTempData();
        MainUi.Instance.xpUI.loadBonus();
        Stats.Instance.AddIron(new BigNumber(0));
        Stats.Instance.AddUranium(new BigNumber(0));
        
        if(life.isBigger(lifeMax.getTotal()))
            life.Set(lifeMax.getTotal());
        if (shield.isBigger(shieldMax.getTotal()))
            shield.Set(shieldMax.getTotal());
    }

    private void LoadMachines()
    {
        List<machineIronElement> machinesIron = new List<machineIronElement>();
        machinesIron.Add(new machineIronElement("Anvil", new BigNumber(10)));
        machinesIron.Add(new machineIronElement("ironMachine", new BigNumber(1, 3)));
        machinesIron.Add(new machineIronElement("ironMachines", new BigNumber(1, 6)));
        machinesIron.Add(new machineIronElement("usine", new BigNumber(1, 9)));
        machinesIron.Add(new machineIronElement("usines", new BigNumber(1, 12)));
        Utility.AddMachineToData(machinesIron, Ship.Current.machineIron);

        List<machineUraniumElement> machinesUranium = new List<machineUraniumElement>();
        machinesUranium.Add(new machineUraniumElement("Anvil", new BigNumber(10)));
        machinesUranium.Add(new machineUraniumElement("ironMachine", new BigNumber(5, 3)));
        machinesUranium.Add(new machineUraniumElement("ironMachines", new BigNumber(5, 6)));
        machinesUranium.Add(new machineUraniumElement("usine", new BigNumber(5, 9)));
        machinesUranium.Add(new machineUraniumElement("usines", new BigNumber(5, 12)));
        Utility.AddMachineToData(machinesUranium, Ship.Current.machinesUranium);
    }

    private void LoadUpgrades()
    {
        List<UpgradesElement> upgradesIron = new List<UpgradesElement>();
        foreach(UpgradesIronElement.UpgradeType type in Enum.GetValues(typeof(UpgradesIronElement.UpgradeType))){
            upgradesIron.Add(new UpgradesIronElement(type.ToString(), type));
        }
        Utility.AddMachineToData(upgradesIron, Ship.Current.upgradesIron);

        List<UpgradesElement> upgradesUranium = new List<UpgradesElement>();
        foreach (UpgradesUraniumElement.UpgradeType type in Enum.GetValues(typeof(UpgradesUraniumElement.UpgradeType)))
        {
            upgradesUranium.Add(new UpgradesUraniumElement(type.ToString(), type));
        }
        Utility.AddMachineToData(upgradesUranium, Ship.Current.upgradesUranium);
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
        foreach (var up in Stats.Instance.upgradesShip)
        {
            up.SetReward();
        }
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

    #endregion

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