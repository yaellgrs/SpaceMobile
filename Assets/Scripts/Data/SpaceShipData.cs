using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;


public enum SpaceShipType { Basic, Fire, Ice };

[Serializable]
public class SpaceShipDico //pour pouvoir serialiser le dictionnaire
{
    public SpaceShipType type;
    public SpaceShipData data;
}

[System.Serializable]
public class SpaceShipData
{

    public int level = 1;

    public List<machineIronElement> machineIron = new List<machineIronElement>();
    public List<machineUraniumElement> machinesUranium = new List<machineUraniumElement>();

    public List<UpgradesElement> upgradesIron = new List<UpgradesElement>();
    public List<UpgradesElement> upgradesUranium = new List<UpgradesElement>();


    [JsonIgnore] public DamageData damage;


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
    }

    private void LoadMachines()
    {
        List<machineIronElement> machinesIron = new List<machineIronElement>();
        machinesIron.Add(new machineIronElement("Anvil", new BigNumber(10), 1f));
        machinesIron.Add(new machineIronElement("ironMachine", new BigNumber(1, 3), 5f));
        machinesIron.Add(new machineIronElement("ironMachines", new BigNumber(1, 6), 15f));
        machinesIron.Add(new machineIronElement("usine", new BigNumber(1, 9), 30f));
        machinesIron.Add(new machineIronElement("usines", new BigNumber(1, 12), 60f));
        AddMachineToData(machinesIron, Ship.Current.machineIron);

        List<machineUraniumElement> machinesUranium = new List<machineUraniumElement>();
        machinesUranium.Add(new machineUraniumElement("Anvil", new BigNumber(0), 3f));
        machinesUranium.Add(new machineUraniumElement("ironMachine", new BigNumber(5, 3), 10f));
        machinesUranium.Add(new machineUraniumElement("ironMachines", new BigNumber(5, 6), 25f));
        machinesUranium.Add(new machineUraniumElement("usine", new BigNumber(5, 9), 45f));
        machinesUranium.Add(new machineUraniumElement("usines", new BigNumber(5, 12), 100f));
        AddMachineToData(machinesUranium, Ship.Current.machinesUranium);
    }

    private void LoadUpgrades()
    {
        List<UpgradesElement> upgradesIron = new List<UpgradesElement>();
        upgradesIron.Add(new UpgradesIronElement("life", UpgradesIronElement.UpgradeType.Life));
        upgradesIron.Add(new UpgradesIronElement("Damage", UpgradesIronElement.UpgradeType.Damage));
        upgradesIron.Add(new UpgradesIronElement("WorldSize", UpgradesIronElement.UpgradeType.WorldSize));
        upgradesIron.Add(new UpgradesIronElement("Shield", UpgradesIronElement.UpgradeType.Shield));
        upgradesIron.Add(new UpgradesIronElement("RegenShield", UpgradesIronElement.UpgradeType.RegenShield));
        AddMachineToData(upgradesIron, Ship.Current.upgradesIron);

        List<UpgradesElement> upgradesUranium = new List<UpgradesElement>();
        upgradesUranium.Add(new UpgradesUraniumElement("SpeedAuto", UpgradesUraniumElement.UpgradeType.SpeedAuto));
        upgradesUranium.Add(new UpgradesUraniumElement("AreaSlow", UpgradesUraniumElement.UpgradeType.AreaSlow));
        upgradesUranium.Add(new UpgradesUraniumElement("AreaWidth", UpgradesUraniumElement.UpgradeType.AreaWidth));
        upgradesUranium.Add(new UpgradesUraniumElement("RocketReload", UpgradesUraniumElement.UpgradeType.RocketReload));
        upgradesUranium.Add(new UpgradesUraniumElement("RocketMultiplier", UpgradesUraniumElement.UpgradeType.RocketMultiplier));
        AddMachineToData(upgradesUranium, Ship.Current.upgradesUranium);
    }

    private void AddMachineToData<T>(List<T> init, List<T> data)
    {
        foreach (T m in init)
        {
            if (!data.Contains(m))
                data.Add(m);
        }
    }

    public void InitTempData()
    {
        damage = new DamageData();

        LoadBonus();
    }

    public void LoadBonus()
    {
        foreach(var upgrade in upgradesIron)
            upgrade.GetReward();
        foreach (var upgrade in upgradesUranium)
            upgrade.GetReward();
        foreach (var upgrade in Stats.Instance.upgradesPrestige)
            upgrade.GetReward();
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



public class DamageData
{
    //stocker ailleurs qu'ici ou stats pour ne pas sauvegarder cette donné, c'est inutile de la sauvegarder
    public BigNumber damageBase;
    public float prestige_multiplicator = 1f;
    public float rocket_multiplicator = 1f;
    public int critical_multiplicator = 5;
    public BigNumber getDamage(bool isRocket, bool critical)
    {
        float leveBonus = 1f + (Ship.Current.level - 1) * 0.1f;
        BigNumber damage = new BigNumber(damageBase) * prestige_multiplicator * leveBonus;
        if(isRocket) damage *= rocket_multiplicator;
        if(critical) damage *= critical_multiplicator;
        return damage;
    }
    /*
                     dmg.Multiply(Stats.Instance.damage_Multiplicator_Lvl);
                dmg.Multiply(Stats.Instance.perm_Damage_Multiplicator_Lvl);
                if (Stats.Instance.damageBoostTime > 0) dmg.Multiply(2);
     */

}




/*
 Stats

enum spaceShiptype;

spaceShiptype curentShip( enum ? ) 
Dictionnaire<spaceShiptype, SpaceShipData>{
	//monnaie des spaceShip 
	//levels des spaceships

	//upgrades ( List<Upgrades> ) -> stocker les ~données dans des scriptable objects
	//machine ? 
}


SpaceShipManager

*setIcons
 */