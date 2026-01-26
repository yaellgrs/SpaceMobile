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



    public override void InitMachines()
    {
        List<machineUraniumElement> mach = new List<machineUraniumElement>();
        mach.Add(new machineUraniumElement("Anvil", new BigNumber(0), 3f));
        mach.Add(new machineUraniumElement("ironMachine", new BigNumber(5, 3), 10f));
        mach.Add(new machineUraniumElement("ironMachines", new BigNumber(5, 6), 25f));
        mach.Add(new machineUraniumElement("usine", new BigNumber(5, 9), 45f));
        mach.Add(new machineUraniumElement("usines", new BigNumber(5, 12), 100f));
        foreach (machineUraniumElement m in mach)
        {
            int x = 0;
            foreach (machineUraniumElement machUp in Stats.Instance.machinesUranium)
            {
                if (m.machineName == machUp.machineName)
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

    public override void InitUpgrades()
    {
        List<UpgradesElement> mach = new List<UpgradesElement>();
        mach.Add(new UpgradesUraniumElement("SpeedAuto", UpgradesUraniumElement.UpgradeType.SpeedAuto));
        mach.Add(new UpgradesUraniumElement("AreaSlow", UpgradesUraniumElement.UpgradeType.AreaSlow));
        mach.Add(new UpgradesUraniumElement("AreaWidth", UpgradesUraniumElement.UpgradeType.AreaWidth));
        mach.Add(new UpgradesUraniumElement("RocketReload", UpgradesUraniumElement.UpgradeType.RocketReload));
        mach.Add(new UpgradesUraniumElement("RocketMultiplier", UpgradesUraniumElement.UpgradeType.RocketMultiplier));

        foreach (UpgradesElement m in mach)
        {
            int x = 0;
            foreach (UpgradesElement machUp in Stats.Instance.upgradesUranium)
            {
                if (m.name == machUp.name)
                {
                    x = 1;
                }
            }
            if (x == 0)
            {
                Stats.Instance.upgradesUranium.Add(m);
            }
        }
    }

    public override void initializeUpgrade()
    {/*
        List<UpgradesUranium> ups = new List<UpgradesUranium>();
        UpgradesUranium upgrade1 = new UpgradesUranium()
        {
            upgradeName = "upgrade1",
            upgradeType = UpgradesUranium.UpgradeType2.SpeedAuto,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };

        UpgradesUranium upgrade2 = new UpgradesUranium()
        {
            upgradeName = "upgrade2",
            upgradeType = UpgradesUranium.UpgradeType2.AreaSlow,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };

        UpgradesUranium upgrade3 = new UpgradesUranium()
        {
            upgradeName = "upgrade3",
            upgradeType = UpgradesUranium.UpgradeType2.AreaWidth,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };
        UpgradesUranium upgrade4 = new UpgradesUranium()
        {
            upgradeName = "upgrade4",
            upgradeType = UpgradesUranium.UpgradeType2.RocketReload,
            levelCostMachine1 = new BigNumber(10, 0),
            machineLevelMax1 = 100
        };
        UpgradesUranium upgrade5 = new UpgradesUranium()
        {
            upgradeName = "upgrade5",
            upgradeType = UpgradesUranium.UpgradeType2.RocketMultiplier,
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
        }*/
    }

    protected override void Update()
    {
        base.Update();
        upUraniumLabel();

        foreach(machineUraniumElement machine in Stats.Instance.machinesUranium)
        {
            machine.Update();
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

        ScrollView scroll = root.Q<ScrollView>("scroll");
        foreach (machineElement machine in Stats.Instance.machinesUranium)
        {
            scroll.Add(machine);
        }

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

        if (XpUI.rewardUnlocked(XpUI.BonusLevel.UnlockUranium))
        {
            uraniumUnlockedVE.style.visibility = Visibility.Hidden;

            uraniumLabel = root.Q<Label>("uranium");
            upUraniumLabel();
            uraniumLabel = root.Q<Label>("uranium");

            foreach (machineUraniumElement machine in Stats.Instance.machinesUranium)
            {
                machine.LoadMachine();
            }
        }
        else
        {
            uraniumUnlockedVE.style.visibility = Visibility.Visible;
        }



        if (!Stats.Instance.uraniumTuto && XpUI.rewardUnlocked(XpUI.BonusLevel.UnlockUranium))
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

        ScrollView scroll = root.Q<ScrollView>("scroll");
        scroll.Clear();
        foreach (UpgradesElement machine in Stats.Instance.upgradesUranium)
        {
            scroll.Add(machine);
            Debug.Log("Add : " + machine.name);
        }

        foreach (UpgradesElement upgrade in Stats.Instance.upgradesUranium)
        {
            upgrade.Load();
        }

        prestigeButton.clicked += prestigeClicked;
        ironButton.clicked += ironClicked;
    }
}
