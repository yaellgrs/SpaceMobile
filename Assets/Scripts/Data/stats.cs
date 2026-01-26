using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using static UpgradePrestige.UpgradeType;

/*
    - supprimer les class machinesiron et uranium et machines.
 */


[System.Serializable]
public class Stats
{
    public int version = 1;

    public static Stats Instance;

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


    //global
    public int diamand { get; private set; } = 1;

    public long lastConnection;
    public bool firstConnection = true;
    public int deadPubWatch = 0;
    public long lastPub = 0;
    public bool HasNoAds = false;

        //boost
    public float damageBoostTime = 0f;
    public float xpBoostTime = 0f;
    public float pvShieldBoostTime = 0f;
    public float ressourcesBoostTime = 0f;

        //tutos
    public bool ironTuto = false;
    public bool uraniumTuto = false;
    public Dictionary<PopupTuto, bool> popupTutos = new Dictionary<PopupTuto, bool>();


    //Ship
    public int stage = 1;
    public int level = 1;

    public BigNumber BN_xp { get; private set; } = new BigNumber(0);
    public BigNumber BN_xpMax = new BigNumber(100);

    public bool isDead = false;
    

        //iron


    public BigNumber iron { get; private set; } = new BigNumber(1, 0);
    public BigNumber uranium { get; private set; } = new BigNumber(0, 0);

    public BigNumber life = new BigNumber(10);
    public BigNumber lifeMax = new BigNumber(10);
    public BigNumber shield = new BigNumber(5);
    public BigNumber shieldMax = new BigNumber(5);
    public BigNumber regenShield = new BigNumber(2);



    //machines upgrades
    //public List<MachineIron> machinesIron = new List<MachineIron>();
    //public List<machineUranium> machinesUranium = new List<machineUranium>();
    public List<machineIronElement> machineIron = new List<machineIronElement>();
    public List<machineUraniumElement> machinesUranium = new List<machineUraniumElement>();

    //public List<UpgradesIron> upgradesIron = new List<UpgradesIron>();
    public List<UpgradesElement> upgradesIronv2 = new List<UpgradesElement>();
    public List<UpgradesUranium> upgradesUranium = new List<UpgradesUranium>();
    public List<UpgradePrestige> upgradesPrestige = new List<UpgradePrestige>();

    public float scale = 1f;
    public float speedAuto = 5f;
    public float areaSpeed = 1f;
    public float areaSize = 1.25f;
    public float rocketTimerMax = 25f;
    public float rocketMultiplier = 5f;

        //prestige

    public bool prestigeUnlocked = false;

    public BigNumber starPariticul { get; private set; } = new BigNumber(1, 0);
    public BigNumber prestigeWaiting { get; private set; } = new BigNumber(1, 0);
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

    public void AddXP(BigNumber amount)
    {
        BN_xp += amount;
        if (BN_xp > BN_xpMax)
        {
            MainUi.Instance.xpUI.LevelUp();
        }
        MainUi.Instance.upLevelUI();
    }

    public void AddDiamand(int amount)
    {
        diamand += amount;
        MainUi.Instance.upDiamandUI();
    }

    public void AddIron(BigNumber amount)
    {
        iron += amount;
        MainUi.Instance.upIronUI();
    }

    public void AddUranium(BigNumber amount)
    {
        uranium += amount;
        MainUi.Instance.upUraniumUI();
    }

    public void addPrestige(BigNumber amount)
    {
        starPariticul += amount;
        MainUi.Instance.prestigeUI.upPrestigeLabel();
    }

    public void AddPrestigeWainting(BigNumber amount)
    {
        prestigeWaiting += amount;
        MainUi.Instance.prestigeUI.upPrestigeLabel();
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
        Instance.life.Set(spaceShip.instance.getMaxLife());
        Instance.shield.Set(spaceShip.instance.getMaxShield());
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
        MainUi.Instance.upStage();
    }
}

