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

    protected override void Start()
    {
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        prestigeUI.gameObject.SetActive(false);
        buyUI.gameObject.SetActive(false);
    }


    public void addNewUpgrades(int prestige)
    {
        Stats.Instance.upPrestige(calculCostPrestige(), false);
        string name = "upgrade" + (Stats.Instance.upgradesPrestige.Count + 1);
        UpgradePrestige.UpgradeType type;
        if (prestige == 1)
        {
            type = Stats.Instance.nextPrestigeToBuy;
        }
        else
        {
            type = Stats.Instance.nextPrestigeToBuy2;
        }
        UpgradePrestige upgrade1 = new UpgradePrestige()
        {
            upgradeName = name,
            upgradeType = type,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };
        upgrade1.loadUpgrade(forgeUI);
        upgrade1.cadre.style.visibility = Visibility.Visible;

        upgrade1.cadre.style.translate = new Translate(0, 215f * (Stats.Instance.upgradesPrestige.Count), 0);
        Stats.Instance.upgradesPrestige.Add(upgrade1);
        buyButtonUI.style.translate = new Translate(0, 215f * (Stats.Instance.upgradesPrestige.Count), 0);

        Stats.Instance.prestigeToBuy.Remove(Stats.Instance.nextPrestigeToBuy);

        if (Stats.Instance.prestigeToBuy.Count == 0)
        {
            buyButton.enabledSelf = false;
            Stats.Instance.nextPrestigeToBuy = UpgradePrestige.UpgradeType.Max;
        }
        else
        {
            setNextPrestigeToBuy();
            setNextPrestigeToBuy2();
        }
        buyUI.gameObject.SetActive(false);
        forgeUI.gameObject.SetActive(false);
        loadForgeUI();
    }

    public static void setNextPrestigeToBuy()
    {
        UpgradePrestige.UpgradeType type = Stats.Instance.prestigeToBuy[Random.Range(0, Stats.Instance.prestigeToBuy.Count)];
        if(Stats.Instance.prestigeToBuy.Count > 1)
        {
            while (Stats.Instance.nextPrestigeToBuy == type)
            {
                if (type == UpgradePrestige.UpgradeType.Max)
                    continue;
                type = Stats.Instance.prestigeToBuy[Random.Range(0, Stats.Instance.prestigeToBuy.Count)];
            }
        }
        else
        {
            type = Stats.Instance.prestigeToBuy[0];
        }

        Stats.Instance.nextPrestigeToBuy = type;
    }

    public static void setNextPrestigeToBuy2()
    {
        UpgradePrestige.UpgradeType type = Stats.Instance.prestigeToBuy[Random.Range(0, Stats.Instance.prestigeToBuy.Count)];
        if (Stats.Instance.prestigeToBuy.Count > 1)
        {
            while (Stats.Instance.nextPrestigeToBuy2 == type || type == Stats.Instance.nextPrestigeToBuy)
            {
                if (type == UpgradePrestige.UpgradeType.Max)
                    continue;
                type = Stats.Instance.prestigeToBuy[Random.Range(0, Stats.Instance.prestigeToBuy.Count)];
            }
        }
        else
        {
            type = Stats.Instance.prestigeToBuy[0];
        }

        Stats.Instance.nextPrestigeToBuy2 = type;
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
        foreach (UpgradePrestige upgrade in Stats.Instance.upgradesPrestige)
        {
            upgrade.update();
        }
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
        forgeUiVE = root.Q<VisualElement>("forgeUI");

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


            prestigeButton.clicked += LoadPrestige;

            int i = 0;
            foreach (UpgradePrestige upgrade in Stats.Instance.upgradesPrestige)
            {
                upgrade.loadUpgrade(forgeUI);
                upgrade.cadre.style.translate = new Translate(0, 215f * i, 0);
                i++;
            }
            upPrestigeLabel();
            buyButtonUI.style.translate = new Translate(0, 215f * (Stats.Instance.upgradesPrestige.Count), 0);
            if (Stats.Instance.prestigeToBuy.Count == 0)
            {
                buyButtonUI.enabledSelf = false;
            }
            else
            {
                buyButtonUI.clicked += LoadBuy;
            }
        }
        else
        {
            unlockLevel.style.visibility = Visibility.Visible;
        }

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
        backButton1.clicked += backClicked;
        prestigeReset.clicked += PrestigeResetClicked;
        if(Stats.Instance.diamand >= 50)
        {
            diamandBtn.enabledSelf = true;
            diamandBtn.clicked += diamandClicked;
        }
        else
        {
            diamandBtn.enabledSelf = false;
        }

    }
    private void diamandClicked()
    {
        Stats.Instance.upPrestige(bonus, true);
        Stats.Instance.upDiamand(50, false);
        PrestigeResetClicked();
    }

    private void PrestigeResetClicked()
    {
        Stats.Instance.upPrestige(bonus, true);
        Stats.Instance.prestigeWaiting = new BigNumber(0);

        Stats.Instance.stage = 1;
        Stats.Instance.machinesIron.Clear();
        Stats.Instance.upgradesIron.Clear();
        Stats.Instance.machinesUranium.Clear();
        Stats.Instance.upgradesUranium = new List<UpgradesUranium>();

        MainUi.Instance.ironUI.initializeMachine();
        MainUi.Instance.ironUI.initializeUpgrade();
        MainUi.Instance.uraniumUI.initializeMachine();
        MainUi.Instance.uraniumUI.initializeUpgrade();

        Stats.Instance.iron = new BigNumber(0);
        Stats.Instance.uranium = new BigNumber(0);

        MainUi.Instance.ironUI.upIronRaffinedUi();
        MainUi.Instance.uraniumUI.upUraniumLabel();


        Stats.Instance.life = new BigNumber(Stats.Instance.lifeMax);
        Stats.Instance.shield = new BigNumber(Stats.Instance.shieldMax);

        gameManager.instance.RestartStage();


        Data.Instance.Prestige();

        backClicked();
        MainUi.Instance.xpUI.setBonusAutoFer();
        MainUi.Instance.xpUI.setBonusAutoUranium();

        if(MainUi.Instance.questUI.type == QuestUI.questType.starParticule)
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
        MainUi.Instance.updateStage();
        MainUi.Instance.upShieldBar();
        MainUi.Instance.upHealthBar();
        MainUi.Instance.upUraniumUI();
        upPrestigeLabel();

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

        if (!Stats.Instance.starPariticul.isBigger(cost))
        {
            refreshButton.enabledSelf = false;
            buyButton.enabledSelf = false;
        }
        else
        {
            refreshButton.clicked -= refreshClicked;
            refreshButton.clicked += refreshClicked;
            if (Stats.Instance.prestigeToBuy.Count == 1)
            {
                refreshButton.enabledSelf = false;
                buyButton.enabledSelf = false;
                nameNextPrestige.style.visibility = Visibility.Hidden;
                descriptionNextPrestige.style.visibility = Visibility.Hidden;
            }
            else
            {
                refreshButton.enabledSelf = true;
                buyButton.enabledSelf = true;
                buyButton.clicked -= buyClicked;
                buyButton.clicked += buyClicked;
            }
        }
        backButton2.clicked -= backClicked;
        backButton2.clicked += backClicked;
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
        {
            MainUi.Instance.prestigeUI.buyUI.gameObject.SetActive(false);
        }
    }
    private BigNumber calculCostPrestige()
    {
        return new BigNumber(15*Mathf.Pow(5, Stats.Instance.upgradesPrestige.Count));
    }
    private void backClicked()
    {

        forgeUiVE.RemoveFromClassList("trans");
        forgeUiVE.schedule.Execute(() =>
        {
            forgeUiVE.AddToClassList("trans");
        }).StartingIn(50);
        forgeUiVE.schedule.Execute(() =>
        {
            if (prestigeUI.gameObject.activeSelf)
            {
                prestigeUI.gameObject.SetActive(false);
            }
            else
            {
                buyUI.gameObject.SetActive(false);
            }

            loadForgeUI();
        }).StartingIn(300);

    }

    private void refreshClicked()
    {
        setNextPrestigeToBuy();
        setNextPrestigeToBuy2();
        Stats.Instance.upPrestige(calculCostPrestige(), false);
        setTextBuyUI(Stats.Instance.nextPrestigeToBuy);
        LastPrestigeClicked();
    }
    //{ , , , , , , StageSkip, , Max }
    private void setTextBuyUI(UpgradePrestige.UpgradeType type)
    {
        string key = "";
        switch (type)
        {
            case UpgradePrestige.UpgradeType.PrestigeMultiplicator:
                key = "PrestigeMultiplicator";
                break;
            case UpgradePrestige.UpgradeType.LessMeteor:
                key = "LessMeteor";
                break;
            case UpgradePrestige.UpgradeType.LessTimeMachine:
                key = "LessTimeMachine";
                break;
            case UpgradePrestige.UpgradeType.LessPriceUpgrades:
                key = "LessPriceUpgrades";
                break;
            case UpgradePrestige.UpgradeType.XpBoost:
                key = "XpBoost";
                break;
            case UpgradePrestige.UpgradeType.DamageMultiplicator:
                key = "DamageMultiplicator";
                break;
            case UpgradePrestige.UpgradeType.OmegaProb:
                key = "OmegaProb";
                break;
            case UpgradePrestige.UpgradeType.StageSkip:
                key = "StageSkip";
                break;
            case UpgradePrestige.UpgradeType.Max:
                nameNextPrestige.text = "Max";
                break;
        }
        if (type != UpgradePrestige.UpgradeType.Max)
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
        forgeUiVE = root.Q<VisualElement>("updateUI");
        black = root.Q<VisualElement>("black");

        uraniumButton.clicked += uraniumClicked;
        ironButton.clicked += ironClicked;
    }
}
