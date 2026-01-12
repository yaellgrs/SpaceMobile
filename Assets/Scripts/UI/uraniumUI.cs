using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UraniumUI : BaseUI
{
    
    private Button ironButton;
    private Button prestigeButton;
    private Button unlockButton;
    private VisualElement uraniumUnlockedVE;
    private Label uraniumLabel;



    public override void initializeMachine()
    {
        List<machineUranium> mach = new List<machineUranium>();
        machineUranium machine5 = new machineUranium()
        {
            name = "machine5",
            machineNumber = 5,
            isVisible = false,
            isActive = false,
            timeMax = 100f,
            BN_levelCost = new BigNumber(2, 9),
            BN_earn = new BigNumber(1, 8),
            BN_initEarn = new BigNumber(1, 8),
            BN_price = new BigNumber(5, 12),
            automatic = false
        };
        machineUranium machine4 = new machineUranium()
        {
            name = "machine4",
            machineNumber = 4,
            isVisible = false,
            isActive = false,
            timeMax = 45f,
            BN_levelCost = new BigNumber(4, 6),
            BN_earn = new BigNumber(5, 5),
            BN_initEarn = new BigNumber(5, 5),
            BN_price = new BigNumber(5, 9),
            automatic = false
        };
        machineUranium machine3 = new machineUranium()
        {
            nextMachine = "machine4",
            name = "machine3",
            machineNumber = 3,
            isVisible = false,
            isActive = false,
            timeMax = 20f,
            BN_levelCost = new BigNumber(17.5f, 5),
            BN_earn = new BigNumber(2, 4),
            BN_initEarn = new BigNumber(2, 4),
            BN_price = new BigNumber(5, 6),
            automatic = false
        };

        machineUranium machine2 = new machineUranium()
        {
            nextMachine = "machine3",
            name = "machine2",
            machineNumber = 2,
            isActive = false,
            isVisible = true,
            timeMax = 10f,
            BN_levelCost = new BigNumber(2, 3),
            BN_earn = new BigNumber(2f, 2),
            BN_initEarn = new BigNumber(2f, 2),
            BN_price = new BigNumber(5, 3),
            automatic = false
        };

        machineUranium machine1 = new machineUranium()
        {
            nextMachine = "machine2",
            name = "machine1",
            machineNumber = 1,
            isActive = true,
            isVisible = true,
            timeMax = 3f,
            BN_levelCost = new BigNumber(25, 0),
            BN_earn = new BigNumber(1, 0),
            BN_initEarn = new BigNumber(1, 0),
            BN_price = new BigNumber(20, 0),
            automatic = false
        };

        mach.Add(machine1);
        mach.Add(machine2);
        mach.Add(machine3);
        mach.Add(machine4);
        mach.Add(machine5);
        foreach (machineUranium m in mach)
        {
            int x = 0;
            foreach (machineUranium machUp in Stats.Instance.machinesUranium)
            {
                if (m.name == machUp.name)
                {
                    x = 1;
                }
            }
            if (x == 0)
            {
                Stats.Instance.machinesUranium.Add(m);
            }
        }
    }

    public override void initializeUpgrade()
    {
        List<UpgradesUranium> ups = new List<UpgradesUranium>();
        UpgradesUranium upgrade1 = new UpgradesUranium()
        {
            upgradeName = "upgrade1",
            upgradeType = UpgradesUranium.UpgradeType.SpeedAuto,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };

        UpgradesUranium upgrade2 = new UpgradesUranium()
        {
            upgradeName = "upgrade2",
            upgradeType = UpgradesUranium.UpgradeType.AreaSlow,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };

        UpgradesUranium upgrade3 = new UpgradesUranium()
        {
            upgradeName = "upgrade3",
            upgradeType = UpgradesUranium.UpgradeType.AreaWidth,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };
        UpgradesUranium upgrade4 = new UpgradesUranium()
        {
            upgradeName = "upgrade4",
            upgradeType = UpgradesUranium.UpgradeType.RocketReload,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };
        UpgradesUranium upgrade5 = new UpgradesUranium()
        {
            upgradeName = "upgrade5",
            upgradeType = UpgradesUranium.UpgradeType.RocketMultiplier,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };

        ups.Add(upgrade1);
        ups.Add(upgrade2);
        ups.Add(upgrade3);
        ups.Add(upgrade4);
        ups.Add(upgrade5);
        foreach(UpgradesUranium up in ups)
        {
            int x = 0;
            foreach(UpgradesUranium StatsUp in Stats.Instance.upgradesUranium)
            {
                if(up.upgradeName == StatsUp.upgradeName)
                {
                    x = 1;
                }
            }
            if(x == 0)
            {
                Stats.Instance.upgradesUranium.Add(up);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        upUraniumLabel();

        foreach(Machine machine in Stats.Instance.machinesUranium)
        {
            machine.machineUpdate();
        }
        foreach(Upgrades up in Stats.Instance.upgradesUranium)
        {
            up.update();
        }
    }

    public void upUraniumLabel()
    {
        if(uraniumLabel != null)
        {
            uraniumLabel.text = Stats.Instance.uranium.ToString();
        }
       
    }

    private void ironClicked()
    {
        Debug.Log("iron clicked");
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.ironUI.gameObject.SetActive(true);
        MainUi.Instance.ironUI.loadForgeUI();

    }

    private void prestigeClicked()
    {

        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.prestigeUI.forgeUI.gameObject.SetActive(true);
        MainUi.Instance.prestigeUI.loadForgeUI();

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

        uraniumLabel = root.Q<Label>("uranium");
        uraniumUnlockedVE = root.Q<VisualElement>("unlockLevel");

        prestigeButton = root.Q<Button>("prestige");
        ironButton = root.Q<Button>("iron");
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


        prestigeButton.clicked += prestigeClicked;
        ironButton.clicked += ironClicked;

        if (Stats.Instance.uraniumUnlocked)
        {
            uraniumUnlockedVE.style.visibility = Visibility.Hidden;

            uraniumLabel = root.Q<Label>("uranium");
            upUraniumLabel();
            uraniumLabel = root.Q<Label>("uranium");

            foreach (Machine machine in Stats.Instance.machinesUranium)
            {
                machine.loadMachine(forgeUI);
            }
        }
        else
        {
            uraniumUnlockedVE.style.visibility = Visibility.Visible;
        }



        if (!Stats.Instance.uraniumTuto && MainUi.Instance.xpUI.rewardUnlocked(XpUI.BonusLevel.UnlockUranium))
        {
            Debug.Log("load uranium tuto");
            Tuto.Instance.LoadForgeTuto(false);
        }
        else
        {
            Debug.Log("uranium tuto already done");
        }
    }

    public override void loadUpdateUI()
    {
        base.loadUpdateUI();
        var root = upgradeUI.rootVisualElement;
        prestigeButton = root.Q<Button>("prestige");
        ironButton = root.Q<Button>("iron");
        uraniumLabel = root.Q<Label>("uranium");
        forgeUiVE = root.Q<VisualElement>("forgeUI");
        upUraniumLabel();

        foreach (UpgradesUranium upgrade in Stats.Instance.upgradesUranium)
        {
            upgrade.loadUpgrade(upgradeUI);
        }

        prestigeButton.clicked += prestigeClicked;
        ironButton.clicked += ironClicked;
    }
}
