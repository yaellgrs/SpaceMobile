using NUnit.Framework.Constraints;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.SmartFormat.Utilities;

public class PrestigeUI : BaseUI
{

    public UIDocument prestigeUI;
    public UIDocument buyUI;

    public UIDocument upgradeShip;


    private Button uraniumButton;
    private Button ironButton;
    private Button prestigeButton;
    private Button buyButtonUI;
    private VisualElement unlockLevel;

    private Label prestigeLabel;
    //prestige UI
    private Button backButton1;
    private Button prestigeReset;
    private Button diamandBtn;
    private Label rewardLabel;
    private Label bonusLabel;
    private Label totalLabel;

    private BigNumber bonus;

    //buy UI
    private Button backButton2;
    private Button buyButton;
    private Button refreshButton;
    private Button nextPrestige;
    private Button lastPrestige;
    private Label nameNextPrestige;
    private Label descriptionNextPrestige;
    private Label costLabel;

    private LocalizedString localizesName;
    private LocalizedString localizesDescription;

    private int prestigeSelected = 1;


    //shipUI
    Label Lbl_shipMoney;

    Button Btn_back;
    Button Btn_buy;
    Label Lbl_cost;

    protected override void Start()
    {
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        prestigeUI.gameObject.SetActive(false);
        buyUI.gameObject.SetActive(false);
        upgradeShip.gameObject.SetActive(false);
    }

    protected override void upModeButtonClicked()
    {
        base.upModeButtonClicked();
        if (forgeUI.gameObject.activeInHierarchy)
        {
            foreach (UpgradesElement up in Stats.Instance.upgradesPrestige)
                up.Load();
        }
/*      else
        {
            foreach (UpgradesElement upgrade in Ship.Current.upgradesIron)
                upgrade.Load();
        }*/
    }


    public void addNewUpgrades(int prestige)
    {
        Stats.Instance.AddUranium(-calculCostPrestige());
        
        UpgradeType type;
        if (prestige == 1)
            type = Stats.Instance.nextPrestigeToBuy;
        else
            type = Stats.Instance.nextPrestigeToBuy2;

        Stats.Instance.upgradesPrestige.Add(new UpgradesPrestigeElement(type.ToString(), type));

        if (prestige == 1)
            Stats.Instance.prestigeToBuy.Remove(Stats.Instance.nextPrestigeToBuy);
        else
            Stats.Instance.prestigeToBuy.Remove(Stats.Instance.nextPrestigeToBuy2);

        if (Stats.Instance.prestigeToBuy.Count == 0)
        {
            buyButton.enabledSelf = false;
            Stats.Instance.nextPrestigeToBuy = UpgradeType.Max;
        }
        else
            SetNextPrestigesToBuy();
        buyUI.gameObject.SetActive(false);
        forgeUI.gameObject.SetActive(false);
        loadForgeUI();
    }

    private void SetNextPrestigesToBuy()
    {
        var list = new List<UpgradeType>(Stats.Instance.prestigeToBuy);
        list.Remove(UpgradeType.Max);

        if (list == null || list.Count <= 1){
            Stats.Instance.nextPrestigeToBuy2 = UpgradeType.Max;
            Stats.Instance.nextPrestigeToBuy = (list.Count == 1) ? list[0] : UpgradeType.Max;
            return;
        }
        else
        {
            UpgradeType first = list[Random.Range(0, list.Count)]; ;
            list.Remove(first);
            Stats.Instance.nextPrestigeToBuy = first;
            Stats.Instance.nextPrestigeToBuy2 = list[Random.Range(0, list.Count)]; ;
        }
    }

    private void uraniumClicked()
    {
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.uraniumUI.gameObject.SetActive(true);
        MainUi.Instance.uraniumUI.loadForgeUI();
    }

    private void ironClicked()
    {

        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.ironUI.forgeUI.gameObject.SetActive(true);
        MainUi.Instance.ironUI.loadForgeUI();
    }

    public void upPrestigeUI()
    {

    }
    protected override void Update()
    {
        base.Update();
    }

    public void upPrestigeLabel()
    {
        if (prestigeLabel != null)
        {
            string prest = Stats.Instance.starPariticul.ToString() + "(+" + Stats.Instance.prestigeWaiting.ToString() + ")";
            prestigeLabel.text = prest;
            prestigeLabel.style.fontSize = 80 - ( prest.Length * 3 );
        }
    }

    public override void IronClicked()
    {

        if (forgeUI.gameObject.activeInHierarchy || upgradeUI.gameObject.activeInHierarchy)
        {
            if(forgeUI.gameObject.activeInHierarchy)forgeUiVE = forgeUI.rootVisualElement.Q<VisualElement>("main");
            if(upgradeUI.gameObject.activeInHierarchy)forgeUiVE = upgradeUI.rootVisualElement.Q<VisualElement>("main");

            forgeUiVE.RemoveFromClassList("prestigeUITrans");
            forgeUiVE.schedule.Execute(() =>
            {
                forgeUiVE.AddToClassList("prestigeUITrans");
                black.style.visibility = Visibility.Hidden;

            }).StartingIn(50);
            forgeUiVE.schedule.Execute(() =>
            {
                forgeUI.gameObject.SetActive(false);
                upgradeUI.gameObject.SetActive(false);
                gameManager.instance.SetPause(false);

            }).StartingIn(500);
            classActived = true;
        }
        else
        {
            gameManager.instance.SetPause(true);
            loadForgeUI();
        }
    }


    public override void loadForgeUI()
    {
        base.loadForgeUI();
        var root = forgeUI.rootVisualElement;
        uraniumButton = root.Q<Button>("uranium");
        ironButton = root.Q<Button>("iron");
        prestigeButton = root.Q<Button>("prestige");
        unlockLevel = root.Q<VisualElement>("unlockLevel");
        black = root.Q<VisualElement>("black");
        forgeUiVE = root.Q<VisualElement>("main");

        if (classActived)
        {
            classActived = false;
            forgeUiVE.AddToClassList("prestigeUITrans");
        }
        forgeUiVE.schedule.Execute(() =>
        {
            forgeUiVE.RemoveFromClassList("prestigeUITrans");
        }).StartingIn(50);


        uraniumButton.clicked += uraniumClicked;
        ironButton.clicked += ironClicked;

        if (Stats.Instance.prestigeUnlocked)
        {
            unlockLevel.style.visibility = Visibility.Hidden;
            buyButtonUI = root.Q<Button>("buy");
            prestigeLabel = root.Q<Label>("prestigeMoney");

            prestigeButton.clicked -= LoadPrestige;
            prestigeButton.clicked += LoadPrestige;

            ScrollView scroll = root.Q<ScrollView>("scroll");
            scroll.Clear();
            foreach (UpgradesElement upgrade in Stats.Instance.upgradesPrestige)
            {
                scroll.Add(upgrade);
                upgrade.Load();
            }
            scroll.Add(buyButtonUI);

            upPrestigeLabel();
            if (Stats.Instance.nextPrestigeToBuy == UpgradeType.Max && UpgradeType.Max == Stats.Instance.nextPrestigeToBuy2)
            {
                buyButtonUI.enabledSelf = false;
            }
            else
            {
                buyButtonUI.clicked -= LoadBuy;
                buyButtonUI.clicked += LoadBuy;
            }
        }
        else
            unlockLevel.style.visibility = Visibility.Visible;
    }

    public void LoadPrestige()
    {
        prestigeUI.gameObject.SetActive(true);
        var root = prestigeUI.rootVisualElement;

        forgeUiVE = root.Q<VisualElement>("main");
        forgeUiVE.AddToClassList("trans");
        forgeUiVE.schedule.Execute(() =>
        {
            forgeUiVE.RemoveFromClassList("trans");
        }).StartingIn(50);

        backButton1 = root.Q<Button>("back");
        prestigeReset = root.Q<Button>("prestige");
        diamandBtn = root.Q<Button>("diamand");
        rewardLabel = root.Q<Label>("reward");
        bonusLabel = root.Q<Label>("bonus");
        totalLabel = root.Q<Label>("total");

        rewardLabel.text = "Normal Reward : " + Stats.Instance.prestigeWaiting;
        bonus = new BigNumber(Stats.Instance.prestigeWaiting);

        float mult = Stats.Instance.star_mutliplicator_level - 1f;;
        bonus.Multiply(mult);
        bonusLabel.text = "Bonus ( x" + Stats.Instance.star_mutliplicator_level.ToString("F2") + " ) : " + bonus;

        bonus.Add(Stats.Instance.prestigeWaiting);
        totalLabel.text = "Total : " + bonus;

        if (Stats.Instance.prestigeWaiting.EqualZero()){
            prestigeReset.enabledSelf = false;
            diamandBtn.enabledSelf = false;
        }
        else
        {
            prestigeReset.enabledSelf = true;
            prestigeReset.clicked -= PrestigeResetClicked;
            prestigeReset.clicked += PrestigeResetClicked;
            if (Stats.Instance.diamand >= 50)
            {
                diamandBtn.enabledSelf = true;
                diamandBtn.clicked += diamandClicked;
                diamandBtn.clicked -= diamandClicked;
            }
        }


        backButton1.clicked -= () => { backClicked(prestigeUI); };
        backButton1.clicked += () => { backClicked(prestigeUI); };

    }
    private void diamandClicked()
    {
        Stats.Instance.AddUranium(bonus);
        Stats.Instance.AddDiamand(-50);
        PrestigeResetClicked();
    }

    private void PrestigeResetClicked()
    {
        Stats.Instance.AddUranium(bonus);
        Stats.Instance.prestigeWaiting.Set(0);

        Ship.Current.stage = 1;

        Ship.Current.dataMachinesIron.Clear();
        Ship.Current.dataMachinesIron.Clear();

        Ship.Current.machinesUranium.Clear();
        Ship.Current.upgradesUranium.Clear();

        MainUi.Instance.ironUI.initializeUpgrade();
        MainUi.Instance.uraniumUI.initializeUpgrade();

        Ship.Current.iron.Set(0);
        Ship.Current.uranium.Set(0);

        MainUi.Instance.ironUI.upIronRaffinedUi();
        MainUi.Instance.uraniumUI.upUraniumLabel();


        Ship.Current.life.Set(Ship.Current.lifeMax.getTotal());
        Ship.Current.shield.Set(Ship.Current.shieldMax.getTotal());

        gameManager.instance.RestartStage();


        Data.Instance.Prestige();

        backClicked(forgeUI);
        backClicked(buyUI);

        if(QuestManager.Instance.type == QuestType.GetStarParticle)
        {
            QuestStats.Instance.progress = new BigNumber(bonus);
        }
        else
        {
            QuestStats.Instance.progress = new BigNumber(0);
        }

            gameManager.instance.SetPause(true);

        if (ResurectionUI.Instance.resurectionUI.gameObject.activeInHierarchy)
        {
            ResurectionUI.Instance.Close();
            MainUi.Instance.ironUI.forgeUI.gameObject.SetActive(false);
            MainUi.Instance.uraniumUI.forgeUI.gameObject.SetActive(false);
            prestigeUI.gameObject.SetActive(false);
        }
        else
        {

            MainUi.Instance.uraniumUI.loadUpdateUI();
            MainUi.Instance.uraniumUI.IronClicked();

            MainUi.Instance.ironUI.loadUpdateUI();
            MainUi.Instance.ironUI.IronClicked();
            MainUi.Instance.ironUI.loadForgeUI();
            MainUi.Instance.ironUI.forgeUI.gameObject.SetActive(false);
        }

        MainUi.Instance.upIronUI();
        MainUi.Instance.upStage();
        MainUi.Instance.upShieldBar();
        MainUi.Instance.upHealthBar();
        MainUi.Instance.upUraniumUI();
        upPrestigeLabel();
        Data.Instance.PrestigeCount += 1;
        Ship.Current.Load();

        gameManager.instance.InitGame();
    }


    private void LoadBuy()
    {
        buyUI.gameObject.SetActive(true);
        var root = buyUI.rootVisualElement;

        forgeUiVE = root.Q<VisualElement>("main");
        forgeUiVE.AddToClassList("trans");
        forgeUiVE.schedule.Execute(() =>
        {
            forgeUiVE.RemoveFromClassList("trans");
        }).StartingIn(50);

        backButton2 = root.Q<Button>("back");
        buyButton = root.Q<Button>("buy");
        refreshButton = root.Q<Button>("refresh");
        nextPrestige = root.Q<Button>("nextPrestige");
        lastPrestige = root.Q<Button>("lastPrestige");
        nameNextPrestige = root.Q<Label>("name");
        costLabel = root.Q<Label>("cost");
        descriptionNextPrestige = root.Q<Label>("description");

        BigNumber cost = calculCostPrestige();
        costLabel.text = cost.ToString();

        nextPrestige.clicked -= NextPrestigeClicked;
        lastPrestige.clicked -= LastPrestigeClicked;
        nextPrestige.clicked += NextPrestigeClicked;
        lastPrestige.clicked += LastPrestigeClicked;
        LastPrestigeClicked();

        //set logo 

        refreshButton.clicked -= refreshClicked;
        refreshButton.clicked += refreshClicked;
        buyButton.clicked -= buyClicked;
        buyButton.clicked += buyClicked;
        backButton2.clicked -= () => { backClicked(buyUI); }; 
        backButton2.clicked += () => { backClicked(buyUI); }; 

        if (UpgradeType.Max == Stats.Instance.nextPrestigeToBuy2)
        {
            nextPrestige.enabledSelf = false;
        }

        if (!Stats.Instance.starPariticul.isBigger(cost))
        {
            refreshButton.enabledSelf = false;
            buyButton.enabledSelf = false;
        }
        else if (Stats.Instance.nextPrestigeToBuy == UpgradeType.Max)
        {
            refreshButton.enabledSelf = false;
            buyButton.enabledSelf = false;
        }
        else
        {
            refreshButton.enabledSelf = true;
            buyButton.enabledSelf = true;
        }
    }

    private void NextPrestigeClicked()
    {
        prestigeSelected = 2;
        nextPrestige.SetEnabled(false);
        lastPrestige.SetEnabled(true);
        setTextBuyUI(Stats.Instance.nextPrestigeToBuy2);
    }

    private void LastPrestigeClicked()
    {
        prestigeSelected = 1;
        nextPrestige.SetEnabled(true);
        lastPrestige.SetEnabled(false);
        setTextBuyUI(Stats.Instance.nextPrestigeToBuy);
    }

    private void buyClicked()
    {
        addNewUpgrades(prestigeSelected);
        if (MainUi.Instance.prestigeUI.buyUI.gameObject.activeSelf == true)
            MainUi.Instance.prestigeUI.buyUI.gameObject.SetActive(false);
    }
    private BigNumber calculCostPrestige()
    {
        return new BigNumber(15*Mathf.Pow(5, Stats.Instance.upgradesPrestige.Count));
    }
    private void backClicked(UIDocument document)
    {
        if (forgeUiVE == null) return;
        forgeUiVE = document.rootVisualElement.Q<VisualElement>("main");    
        forgeUiVE.RemoveFromClassList("trans");
        forgeUiVE.schedule.Execute(() =>
        {
            forgeUiVE.AddToClassList("trans");
        }).StartingIn(50);
        forgeUiVE.schedule.Execute(() =>
        {
            document.gameObject.SetActive(false);

            //loadForgeUI();
        }).StartingIn(300);

    }

    private void refreshClicked()
    {
        SetNextPrestigesToBuy();
        Stats.Instance.AddUranium(-calculCostPrestige());
        setTextBuyUI(Stats.Instance.nextPrestigeToBuy);
        LastPrestigeClicked();
    }

    private void setTextBuyUI(UpgradeType type)
    {
        VisualElement logo = buyUI.rootVisualElement.Q<VisualElement>("logo");
        string logoPath = "Upgrades/prestige/" ;
        Texture2D logoTexutre = Resources.Load<Texture2D>(logoPath + type);
        if(logoTexutre == null) logoTexutre = Resources.Load<Texture2D>(logoPath + "CadresBlanc");

        logo.style.backgroundImage = logoTexutre;

        string key = type.ToString();
        if (type != UpgradeType.Max)
        {
            string key_name = "Prestige_name_" + key;
            localizesName = new LocalizedString("UI_Rewards", key_name);
            localizesName.StringChanged += (localizedValue) =>
            {
                nameNextPrestige.text = localizedValue;
            };
        }

        string key_descrition = "Prestige_description_" + key;
        localizesDescription = new LocalizedString("UI_Rewards", key_descrition);

        localizesDescription.StringChanged += (localizedValue) =>
        {
            descriptionNextPrestige.text = localizedValue;
        };
    }

    public override void loadUpdateUI()
    {
        base.loadUpdateUI();
        var root = upgradeUI.rootVisualElement;


        uraniumButton = root.Q<Button>("uranium");
        ironButton = root.Q<Button>("iron");
        forgeUiVE = root.Q<VisualElement>("main");
        Button Btn_ship = root.Q<Button>("ship");
        black = root.Q<VisualElement>("black");
        Lbl_shipMoney = root.Q<Label>("shipMoney");

        loadShipMoney();

        ScrollView scroll = root.Q<ScrollView>("scroll");
        scroll.Clear();
        foreach (UpgradesShipElement upgrade in Stats.Instance.upgradesShip)
        {
            if(upgrade.isUnlocked()){
            scroll.Add(upgrade);
                upgrade.Load();
            }
        }
        //scroll.Add(buyButtonUI);

        uraniumButton.clicked += uraniumClicked;
        ironButton.clicked += ironClicked;

        Btn_ship.clicked -= loadUpgradeShip;
        Btn_ship.clicked += loadUpgradeShip;

        Stats.Instance.OnShipMoneyChanged -= loadShipMoney;
        Stats.Instance.OnShipMoneyChanged += loadShipMoney;
    }

    private void loadShipMoney()
    {
        if(Lbl_shipMoney != null) Lbl_shipMoney.text = Stats.Instance.BN_shipMoney.ToString() + " (+" + Stats.Instance.BN_shipMoneyWaiting.ToString() +")";  
    }

    private void loadUpgradeShip()
    {
        upgradeShip.gameObject.SetActive(true);
        var root = upgradeShip.rootVisualElement;


        forgeUiVE = root.Q<VisualElement>("main");

        forgeUiVE.AddToClassList("trans");
        forgeUiVE.schedule.Execute(() =>
        {
            forgeUiVE.RemoveFromClassList("trans");
        }).StartingIn(50);


        Btn_back = root.Q<Button>("back");
        Btn_buy = root.Q<Button>("buy");
        Lbl_cost = root.Q<Label>("cost");

        Btn_back.clicked -= () => { backClicked(upgradeShip); };
        Btn_back.clicked += () => { backClicked(upgradeShip); };

        LoadBuyUI();

        Btn_buy.clicked -= BuyNextShip;
        Btn_buy.clicked += BuyNextShip;
    }

    private void LoadBuyUI()
    {
        bool canBuy = Stats.Instance.shipFragment >= 100;
        Lbl_cost.text = Stats.Instance.shipFragment + "/100";
        Btn_buy.enabledSelf = canBuy;
    }


    private void BuyNextShip()
    {
        Ship.Current.type = (SpaceShipData.SpaceShipElement)Unity.Mathematics.math.clamp((int)Ship.Current.type + 1 ,0,  System.Enum.GetValues(typeof(SpaceShipData.SpaceShipElement)).Length - 1);
        loadUpdateUI();
        backClicked(upgradeShip);
    }

}
