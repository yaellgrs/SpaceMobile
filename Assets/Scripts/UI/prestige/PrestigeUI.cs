using NUnit.Framework.Constraints;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.SmartFormat.Utilities;
using System;

public class PrestigeUI : BaseUI
{
    private static Dictionary<UpgradeType, float> UpgradesPriceFactor = new Dictionary<UpgradeType, float>()
    {
        { UpgradeType.PrestigeMultiplicator, 1.4f},
        { UpgradeType.LessPriceUpgrades, 1.13f},
        { UpgradeType.DamageMultiplicator, 1.125f},
        { UpgradeType.StageSkip, 1.135f},
        { UpgradeType.OmegaProb, 1.15f},
        { UpgradeType.MinimumLevel, 1.165f},

        { UpgradeType.CriticalProbability, 1.4f},
        { UpgradeType.LessMeteor, 1.4f},
        { UpgradeType.XpBoost, 1.4f},
    };

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
    private Label Lbl_boost;
    private Label totalLabel;

    private BigNumber bonus;
    private BigNumber BN_boost;
    private BigNumber BN_reward;

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
        
        UpgradeType type = (prestige == 1) ? Stats.Instance.nextPrestigeToBuy : Stats.Instance.nextPrestigeToBuy2;

        UpgradeData data = new UpgradeData(UpgradesPriceFactor[type]);
        Stats.Instance.dataUpgradePrestige[type] = data;
        Stats.Instance.upgradesPrestige.Add(new UpgradesPrestigeElement(data, type.ToString(), type));


        if (GetPrestigeToBuy().Count == 0)
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

    public List<UpgradeType> GetPrestigeToBuy()
    {
        List<UpgradeType> list = new List<UpgradeType>();
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            if (type != UpgradeType.Max && !Stats.Instance.dataUpgradePrestige.ContainsKey(type))
                list.Add(type);
        }
        return list;
    }

    private void SetNextPrestigesToBuy()
    {
        List<UpgradeType> list = GetPrestigeToBuy();


        if(list.Count <= 0)
        {
            Stats.Instance.nextPrestigeToBuy = UpgradeType.Max;
            Stats.Instance.nextPrestigeToBuy2 = UpgradeType.Max;
            return;
        }
        UpgradeType type1 = list[UnityEngine.Random.Range(0, list.Count)];
        Stats.Instance.nextPrestigeToBuy = type1;
        list.Remove(type1);
        if (list.Count <= 0)
        {
            Stats.Instance.nextPrestigeToBuy2 = UpgradeType.Max;
            return;
        }
        UpgradeType type2 = list[UnityEngine.Random.Range(0, list.Count)];
        Stats.Instance.nextPrestigeToBuy2 = type2;


        //    var list = new List<UpgradeType>(Stats.Instance.prestigeToBuy);
        //    list.Remove(UpgradeType.Max);

        //    if (list == null || list.Count <= 1){
        //        Stats.Instance.nextPrestigeToBuy2 = UpgradeType.Max;
        //        Stats.Instance.nextPrestigeToBuy = (list.Count == 1) ? list[0] : UpgradeType.Max;
        //        return;
        //    }
        //    else
        //    {
        //        UpgradeType first = list[Random.Range(0, list.Count)]; ;
        //        list.Remove(first);
        //        Stats.Instance.nextPrestigeToBuy = first;
        //        Stats.Instance.nextPrestigeToBuy2 = list[Random.Range(0, list.Count)]; ;
        //    }
    }

    private void uraniumClicked()
    {
        if (!Ship.Current.HaveUranium()) return;
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.uraniumUI.gameObject.SetActive(true);
        MainUi.Instance.uraniumUI.loadForgeUI();
        classActived = true;
    }

    private void ironClicked()
    {

        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.ironUI.forgeUI.gameObject.SetActive(true);
        MainUi.Instance.ironUI.loadForgeUI();
        classActived = true;
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
                BottomUI.Instance.OpenMenu(SelectedMenu.None);

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
        BottomUI.Instance.OpenMenu(SelectedMenu.Prestige);

        var root = forgeUI.rootVisualElement;
        uraniumButton = root.Q<Button>("uranium");
        ironButton = root.Q<Button>("iron");
        prestigeButton = root.Q<Button>("prestige");
        unlockLevel = root.Q<VisualElement>("unlockLevel");
        black = root.Q<VisualElement>("black");
        forgeUiVE = root.Q<VisualElement>("main");

        adaptBanner(Settings.Instance.showBanner);

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

        DialogueManager.Instance.TryDialogue("FirstPrestigeOpen");
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
        Lbl_boost = root.Q<Label>("boost");
        totalLabel = root.Q<Label>("total");

        rewardLabel.text = "Normal Reward : " + Stats.Instance.prestigeWaiting;


        float mult = Stats.Instance.star_multiplicator_prestige - 1f;

        bonus = new BigNumber(Stats.Instance.prestigeWaiting);
        BN_reward = new BigNumber(Stats.Instance.prestigeWaiting);
        bonus.Multiply(mult);
        bonusLabel.text = "Bonus ( x" + Stats.Instance.star_multiplicator_prestige.ToString("F2") + " ) : +" + bonus;



        BN_reward.Add(bonus);

        BN_boost = new BigNumber(BN_reward);

        BN_boost.Multiply((Stats.Instance.boosts[Boost.Type.prestige].time <= 0 ? 0f : Stats.Instance.boosts[Boost.Type.prestige].coef - 1));
        Lbl_boost.text = "Boost ( " + ((Stats.Instance.boosts[Boost.Type.prestige].time <= 0) ? "Inactive" 
                        : "x"+Stats.Instance.boosts[Boost.Type.prestige].coef.ToString("F1")) + " ) : +"
                        + BN_boost.ToString();

        BN_reward.Add(BN_boost);

        totalLabel.text = "Total : " + BN_reward;

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
        Stats.Instance.addPrestige(BN_reward);
        Stats.Instance.AddDiamand(-50);
        PrestigeResetClicked();
    }

    private void PrestigeResetClicked()
    {
        Stats.Instance.addPrestige(BN_reward);
        Stats.Instance.prestigeWaiting.Set(0);

        Ship.Current.stage = 1;

        Ship.Current.iron.Set(0);
        Ship.Current.uranium.Set(0);

        MainUi.Instance.ironUI.upIronRaffinedUi();
        MainUi.Instance.uraniumUI.upUraniumLabel();


        Ship.Current.life.Set(Ship.Current.lifeMax.getTotal());
        Ship.Current.shield.Set(Ship.Current.shieldMax.getTotal());

        Ship.Current.Prestige(); // Load(true);

        Datas.Instance.Prestige();

        backClicked(buyUI);
        backClicked(forgeUI);
        backClicked(prestigeUI);




        gameManager.instance.DestroyMeteors();
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
        Ship.Current.Load();

        gameManager.instance.InitGame();

        gameManager.instance.RestartStage();



        if (QuestManager.Instance.type == QuestType.Prestige)
        {
            QuestManager.Instance.upQuest();
        }
        if (QuestManager.Instance.type == QuestType.GetStarParticle)
            QuestManager.Instance.upQuest(BN_reward);

        StartCoroutine(DialogueUI.Instance.LaunchTransition());
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
        Stats.Instance.addPrestige(-calculCostPrestige());
        addNewUpgrades(prestigeSelected);
        if (MainUi.Instance.prestigeUI.buyUI.gameObject.activeSelf == true)
            MainUi.Instance.prestigeUI.buyUI.gameObject.SetActive(false);
    }
    private BigNumber calculCostPrestige()
    {
        return new BigNumber(1*Mathf.Pow(5, Stats.Instance.upgradesPrestige.Count));
    }
    private void backClicked(UIDocument document)
    {
        forgeUiVE = document.rootVisualElement?.Q<VisualElement>("main");
        if(forgeUiVE == null) return;
        forgeUiVE.RemoveFromClassList("trans");
        forgeUiVE.schedule.Execute(() =>
        {
            if (forgeUiVE == null) return;
            forgeUiVE.AddToClassList("trans"); //ici
        }).StartingIn(50);
        forgeUiVE.schedule.Execute(() =>
        {
            if (document == null) return;
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
        black = root.Q<VisualElement>("black");
        Lbl_shipMoney = root.Q<Label>("shipMoney");


        Btn_buy = root.Q<Button>("buy");
        Lbl_cost = root.Q<Label>("cost");

        adaptBanner(Settings.Instance.showBanner);

        VisualElement haveNextLevel = root.Q<VisualElement>("haveNextShip");
        VisualElement isLastShip = root.Q<VisualElement>("isLastShip");
        bool last = Ship.Current != null ? Ship.Current.isLastShip() : false;
        haveNextLevel.style.display = last ? DisplayStyle.None : DisplayStyle.Flex;
        isLastShip.style.display = last ? DisplayStyle.Flex : DisplayStyle.None;

        LoadBuyUI();
        if (last) Btn_buy.style.display = DisplayStyle.None;


        uraniumButton.clicked += uraniumClicked;
        ironButton.clicked += ironClicked;

        Btn_buy.clicked -= loadUpgradeShip;
        Btn_buy.clicked += loadUpgradeShip;
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
        Button Btn_close = root.Q<Button>("close");
        Btn_buy = root.Q<Button>("upgrade");


        Btn_back.clicked -= () => { backClicked(upgradeShip); };
        Btn_back.clicked += () => { backClicked(upgradeShip); };
        Btn_close.clicked -= () => { backClicked(upgradeShip); };
        Btn_close.clicked += () => { backClicked(upgradeShip); };

        Btn_buy.clicked -= BuyNextShip;
        Btn_buy.clicked += BuyNextShip;
    }

    private void LoadBuyUI()
    {
        bool canBuy = QuestStats.Instance.questLevel > QuestStats.Instance.questMaxLevel;
        Lbl_cost.text = (QuestStats.Instance.questLevel - 1) + "/" + QuestStats.Instance.questMaxLevel;
        Btn_buy.SetEnabled(canBuy);
    }


    private void BuyNextShip()
    {
        Debug.Log("clicked");
        Ship.Current.SetNextType(); 

        backClicked(upgradeShip);
        backClicked(upgradeUI);

        spaceShip.instance.LoadAnimation();

        BottomUI.Instance.OpenMenu(SelectedMenu.None);
        gameManager.instance.SetPause(false);

        StartCoroutine(DialogueUI.Instance.LaunchTransition());

    }

}
