using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using static UpgradePrestige.UpgradeType;

[System.Serializable]
public class Stats
{
    public int version = 1;

    public static Stats Instance;

    public int stage = 1;

    public float xp = 0;
    public float xpLevelUp = 100;
    public int level = 1;
    public int diamand = 1;

    public long lastConnection;
    public bool firstConnection = true;
    public int deadPubWatch = 0;
    public long lastPub = 0;
    public bool HasNoAds = false;

    public bool rocketUnlocked = false;

    //iron
    public List<MachineIron> machinesIron = new List<MachineIron>();
    public List<UpgradesIron> upgradesIron = new List<UpgradesIron>();

    public BigNumber iron = new BigNumber(1, 0);
    public BigNumber lifeMax = new BigNumber(10);
    public BigNumber life = new BigNumber(10);
    public BigNumber shield = new BigNumber(5);
    public BigNumber shieldMax = new BigNumber(5);
    public BigNumber regenShield = new BigNumber(2);
    public float scale = 1f;

    //uranium
    public List<machineUranium> machinesUranium = new List<machineUranium>();
    public List<UpgradesUranium> upgradesUranium = new List<UpgradesUranium>();
    public bool uraniumUnlocked = false;

    public BigNumber uranium = new BigNumber(0, 0);
    public float speedAuto = 5f;
    public float areaSpeed = 1f;
    public float areaSize = 1.25f;
    public float rocketTimerMax = 25f;
    public float rocketMultiplier = 5f;

    //prestige
    public List<UpgradePrestige> upgradesPrestige = new List<UpgradePrestige>();
    public bool prestigeUnlocked = false;

    public BigNumber starPariticul = new BigNumber(1, 0);
    public BigNumber prestigeWaiting = new BigNumber(1, 0);
    public float star_multiplicator_prestige = 1f;
    public float enemyPerStage = 10f;
    public float machineTimeReducer = 1f;
    public float upgradesPriceReducer = 1f;
    public float XpMultiplicator = 1f;
    public float prest_damage_multiplicator = 1f;
    public float probabilitéOfOmega = 5f;
    public float stageSkipProb = 0f;

    //levels
    public int diamandProb =5; // / 1000
    public float machineBoost_Lvl = 1f;
    public float star_mutliplicator_level = 1f;
    public float perm_Damage_Multiplicator_Lvl = 1f;
    public float damage_Multiplicator_Lvl = 1f;
    public float life_Multiplicator_Lvl = 1f;
    public float shield_Multiplicator_Lvl = 1f;
    public int SpeedLevel = 1;
    public float offline_Prod_Part = 0.25f;
    public float critical_Prob = 5;
    public float shield_Regen_Time = 4f;

    //boost
    public float damageBoostTime = 0f;
    public float xpBoostTime = 0f;
    public float pvShieldBoostTime = 0f;
    public float ressourcesBoostTime = 0f;

    //tutos
    public bool ironTuto = false;
    public bool uraniumTuto = false;
    //public bool ironMeteorTuto = false;
    public Dictionary<PopupTuto, bool> popupTutos = new Dictionary<PopupTuto, bool>();

    


    public List<UpgradePrestige.UpgradeType> prestigeToBuy = new List<UpgradePrestige.UpgradeType> {
        PrestigeMultiplicator,
        LessMeteor,
        LessTimeMachine,
        LessPriceUpgrades,
        XpBoost,
        DamageMultiplicator,
        StageSkip,
        OmegaProb };
    public UpgradePrestige.UpgradeType nextPrestigeToBuy = DamageMultiplicator;
    public UpgradePrestige.UpgradeType nextPrestigeToBuy2 = PrestigeMultiplicator;

    public static void Initialize()
    {
        if (Instance == null)
        {
            Instance = new Stats();

            float version = Instance.version;
            Instance.load();
            if (Instance.version < version)
            {
                Instance.reset();
            }
        }
    }

    public void upDiamand(int amount, bool positive)
    {
        if (positive) {
            diamand += amount;
        }
        else
        {
            diamand -= amount;
        }
        MainUi.Instance.upDiamandUI();
    }

    public void upIron(BigNumber amount, bool positive)
    {
        if (positive)
        {
            iron.Add(amount);
        }
        else
        {
            iron.Subtract(amount);
        }
        MainUi.Instance.upIronUI();
    }

    public void upUranium(BigNumber amount, bool positive)
    {
        if (positive)
        {
            uranium.Add(amount);
        }
        else
        {
            uranium.Subtract(amount);
        }
        MainUi.Instance.upUraniumUI();
    }

    public void upPrestige(BigNumber amount, bool positive)
    {
        if (positive)
        {
            starPariticul.Add(amount);
        }
        else
        {
            starPariticul.Subtract(amount);
        }
        MainUi.Instance.prestigeUI.upPrestigeLabel();
    }

    public void upPrestigeWaiting(BigNumber amount, bool positive)
    {
        if (positive)
        {
            prestigeWaiting.Add(amount);
        }
        else
        {
            prestigeWaiting.Subtract(amount);
        }
        MainUi.Instance.prestigeUI.upPrestigeLabel();
    }



    public void upLife(BigNumber amount, bool positive)
    {
        
        if (positive)
        {
            life.Add(amount);
        }
        else
        {
            life.Subtract(amount);
        }
        if( new BigNumber(0, 0).isBigger(life))
        {
            life = new BigNumber(0);
        }

    }

    public void upShield(BigNumber amount, bool positive)
    {

        if (positive)
        {
            shield.Add(amount);
        }
        else
        {
            shield.Subtract(amount);
        }
        if (new BigNumber(0, 0).isBigger(shield))
        {
            shield = new BigNumber(0);
        }

    }
    public void upLifeMax(BigNumber amount, bool positive)
    {
        
        if (positive)
        {
            lifeMax.Add(amount);
        }
        else
        {
            lifeMax.Subtract(amount);
        }
    }

    public void load() {
        string path = Application.persistentDataPath + "stats.json";

        if (!System.IO.File.Exists(path))
        {
            return;
        }
        else
        {
            string data = System.IO.File.ReadAllText(path);
            Instance = JsonUtility.FromJson<Stats>(data);
        }
        Instance.life = new BigNumber(spaceShip.instance.getMaxLife());
        Instance.shield = new BigNumber(spaceShip.instance.getMaxShield());
    }

    public void save() {
        if(firstConnection) firstConnection = false;
        lastConnection = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string path = Application.persistentDataPath + "stats.json";
        string stat = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(path, stat);
        QuestStats.Instance.Save();
        Data.Instance.Save();
    }

    public void reset()
    {
        Instance = new Stats();
        save();
        QuestStats.Instance.reset();
        Data.Instance.reset();
    }


}

