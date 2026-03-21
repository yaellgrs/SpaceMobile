using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;

public static class Ship
{
    public static SpaceShipData Current
    {
        get
        {
            if (Stats.Instance == null)
                Stats.Initialize(); // assure l'instance avant d'y accéder
            return Stats.Instance.CurrentSpaceShip;
        }
    }
}

[System.Serializable]
public class Stats
{
    public static Stats Instance;
    public static void Initialize()
    {
        if (Instance == null)
        {
            Instance = new Stats();
            float version = Instance.version;
            Instance.load();
            if (Instance.version < version)
                Instance.reset();

            if (Instance.spaceShips.Count == 0)
            {
                Instance.spaceShips.Add(new SpaceShipDico{
                    type = SpaceShipType.Main,
                    data = new SpaceShipData()
                });
            }
        }
    }

    public int version = 1;

    //constantes
    public const int BOSS_STAGE_GAP = 10;

    //actions 
    public event Action OnIronChanged;
    public event Action OnShipMoneyChanged;

    //ships
    public SpaceShipType currentSpaceShipType = SpaceShipType.Main;
    public List<SpaceShipDico> spaceShips = new List<SpaceShipDico>();
    [JsonIgnore] public SpaceShipData CurrentSpaceShip => spaceShips.Find(e => e.type == currentSpaceShipType)?.data;

    //global
    public int diamand { get; private set; } = 0;

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
    public bool ReduceLifeBoss = false;

    //tutos
    public bool ironTuto = false;
    public bool uraniumTuto = false;
    public Dictionary<PopupTuto, bool> popupTutos = new Dictionary<PopupTuto, bool>();

    //Ship

    //machines upgrades
    public Dictionary<UpgradeType, UpgradeData> dataUpgradePrestige = new Dictionary<UpgradeType, UpgradeData>();
    [JsonIgnore] public List<UpgradesElement> upgradesPrestige = new List<UpgradesElement>();

    public float scale = 1f; 
    public float speedAuto = 5f; 
    public float areaSpeed = 1f; 
    public float areaSize = 1.25f;
    public float rocketTimerMax = 25f;
    public float rocketMultiplier = 5f;

    //prestige
    public bool prestigeUnlocked = false; //

    public BigNumber starPariticul { get; private set; } = new BigNumber(0, 0); //
    public BigNumber prestigeWaiting { get; private set; } = new BigNumber(0, 0);//

    public BigNumber BN_shipMoney = new BigNumber(0);
    public BigNumber BN_shipMoneyWaiting = new BigNumber(0);

    public float star_multiplicator_prestige = 1f;
    public float enemyPerStage = 10f;
    public float machineTimeReducer = 1f;
    public float upgradesPriceReducer = 1f;
    public float XpMultiplicator = 1f;
    public float prest_damage_multiplicator = 1f;
    public float probabilitéOfOmega = 5f;
    public float stageSkipProb = 0f;
    public int MinimalLevel = 1;
    public float critical_Prob = 5;

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

    public float shield_Regen_Time = 4f;

    public Dictionary<UpgradesShipElement.UpgradeType, float> shipUpgradesReward = new Dictionary<UpgradesShipElement.UpgradeType, float>();
    public BigNumber BN_shipUpgradesMoney = new BigNumber(0);

    public int shipFragment = 0;

    public List<UpgradeType> prestigeToBuy = new List<UpgradeType> {
            UpgradeType.PrestigeMultiplicator,
            UpgradeType.LessMeteor,
            UpgradeType.LessPriceUpgrades,
            UpgradeType.XpBoost,
            UpgradeType.DamageMultiplicator,
            UpgradeType.StageSkip,
            UpgradeType.OmegaProb, 
            UpgradeType.MinimumLevel, 
            UpgradeType.CriticalProbability, 
    };

    public UpgradeType nextPrestigeToBuy = UpgradeType.DamageMultiplicator;
    public UpgradeType nextPrestigeToBuy2 = UpgradeType.PrestigeMultiplicator;

    public Stats()
    {
    }

    public static void Init()
    {
        foreach (var data in Instance.dataUpgradePrestige)
            Instance.upgradesPrestige.Add(new UpgradesPrestigeElement(data.Value, data.Key.ToString(), data.Key));

        if (Ship.Current?.lifeMax != null) Ship.Current.life.Set(Ship.Current.lifeMax.getTotal());
        if (Ship.Current?.shieldMax != null) Ship.Current.shield.Set(Ship.Current.shieldMax.getTotal());
    }

    public void AddDiamand(int amount)
    {
        diamand += amount;
        MainUi.Instance.upDiamandUI();
    }

    public void AddShipFragment(int amount)
    {
        shipFragment += amount;
    }

    public void AddShipMoney(BigNumber amount, bool waiting)
    {
        if (waiting) BN_shipMoneyWaiting += amount;
        else BN_shipMoney += amount;
        OnShipMoneyChanged?.Invoke();
    }

    public void AddIron(BigNumber amount)
    {
        Ship.Current.iron += amount;
        MainUi.Instance?.upIronUI();
        Datas.Instance.current.iron += amount;
        OnIronChanged.Invoke();
    }

    public void AddUranium(BigNumber amount)
    {
        Ship.Current.uranium += amount;
        Datas.Instance.current.uranium += amount;
        MainUi.Instance?.upUraniumUI();
    }

    public void addPrestige(BigNumber amount)
    {
        starPariticul += amount;
        Datas.Instance.current.startParticle += amount;
        MainUi.Instance.prestigeUI.upPrestigeLabel();
    }

    public void AddPrestigeWainting(BigNumber amount)
    {
        prestigeWaiting += amount;
        MainUi.Instance.prestigeUI.upPrestigeLabel();
    }
    public void load() {
        string path = Application.persistentDataPath + "/stats.json";

        if (!System.IO.File.Exists(path))
            return;

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        string data = System.IO.File.ReadAllText(path);

        Instance = JsonConvert.DeserializeObject<Stats>(data, settings);
        if(Instance == null)
        {
            Instance = new Stats();
        }

        Init();
    }

    public void save() {
        if(firstConnection) firstConnection = false;
        lastConnection = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string path = Application.persistentDataPath + "/stats.json";

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        string stat = JsonConvert.SerializeObject(Instance, settings);
        System.IO.File.WriteAllText(path, stat);

        QuestStats.Instance.Save();
        Datas.Instance.Save();
    }

    public void reset()
    {
        Instance = new Stats();
        Init();
        ShipManager.Instance.LoadShips();
        save();
        QuestStats.Instance.reset();
        Datas.Instance.reset();
        MainUi.Instance.upStage();
        Tuto.Instance.loadPopupTuto();

        if (Instance.spaceShips.Count == 0)
        {
            Instance.spaceShips.Add(new SpaceShipDico
            {
                type = SpaceShipType.Main,
                data = new SpaceShipData()
            });
        }
    }


}
