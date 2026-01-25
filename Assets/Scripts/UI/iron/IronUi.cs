using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Filters;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class IronUi : BaseUI
{
    private Button uraniumButton;
    private Button prestigeButton;
    private Label ironLabel;

    protected override void Start()
    {
        base.Start();
        loadForgeUI();
        var root = forgeUI.rootVisualElement;
        forgeUiVE.AddToClassList("forgeIronTrans");
        forgeUI.gameObject.SetActive(false);
    }

    public override void InitMachines()
    {
        List<machineIronElement> mach = new List<machineIronElement>();
        mach.Add(new machineIronElement("Anvil", new BigNumber(0), 1f));
        mach.Add(new machineIronElement("ironMachine", new BigNumber(1, 3), 5f));
        mach.Add(new machineIronElement("ironMachines", new BigNumber(1, 6), 15f));
        mach.Add(new machineIronElement("usine", new BigNumber(1, 9), 30f));
        mach.Add(new machineIronElement("usines", new BigNumber(1, 12), 60f));
        foreach (machineIronElement m in mach)
        {
            int x = 0;
            foreach (machineIronElement machUp in Stats.Instance.machineIron)
            {
                if (m.machineName == machUp.machineName)
                {
                    x = 1;
                }
            }
            if (x == 0)
            {
                Stats.Instance.machineIron.Add(m);
            }
        }
    }

    public override void initializeUpgrade()
    {
        List<UpgradesIron> ups = new List<UpgradesIron>();
        UpgradesIron upgrade1 = new UpgradesIron()
        {
            upgradeName = "upgrade1",
            upgradeType = UpgradesIron.UpgradeType.Life,
            levelCostMachine1 = new BigNumber(50, 0),
            machineLevelMax1 = 100
        };

        UpgradesIron upgrade2 = new UpgradesIron()
        {
            upgradeName = "upgrade2",
            upgradeType = UpgradesIron.UpgradeType.Damage,
            levelCostMachine1 = new BigNumber(50, 0),
            machineLevelMax1 = 100
        };
        UpgradesIron upgrade3 = new UpgradesIron()
        {
            upgradeName = "upgrade3",
            upgradeType = UpgradesIron.UpgradeType.WorldSize,
            levelCostMachine1 = new BigNumber(50, 0),
            machineLevelMax1 = 100
        };

        UpgradesIron upgrade4 = new UpgradesIron()
        {
            upgradeName = "upgrade4",
            upgradeType = UpgradesIron.UpgradeType.Shield,
            levelCostMachine1 = new BigNumber(50, 0),
            machineLevelMax1 = 100
        };
        UpgradesIron upgrade5 = new UpgradesIron()
        {
            upgradeName = "upgrade5",
            upgradeType = UpgradesIron.UpgradeType.RegenShield,
            levelCostMachine1 = new BigNumber(50, 0),
            machineLevelMax1 = 100
        };

        ups.Add(upgrade1);
        ups.Add(upgrade2);
        ups.Add(upgrade3);
        ups.Add(upgrade4);
        ups.Add(upgrade5);

        foreach (UpgradesIron up in ups)
        {
            int x = 0;
            foreach (UpgradesIron StatsUp in Stats.Instance.upgradesIron)
            {
                if (up.upgradeName == StatsUp.upgradeName)
                {
                    x = 1;
                }
            }
            if (x == 0)
            {
                Stats.Instance.upgradesIron.Add(up);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        foreach (machineElement machine in Stats.Instance.machineIron)
        {
            machine.Update();
        }
        foreach (Upgrades upgrade in Stats.Instance.upgradesIron)
        {
            upgrade.update();
        }
        upIronRaffinedUi();
    }

    public override void IronClicked()
    {
        if (forgeUI.gameObject.activeInHierarchy || upgradeUI.gameObject.activeInHierarchy)
        {
            string className = "";
            if (forgeUI.gameObject.activeInHierarchy)
            {
               className = "forgeIronTrans";
            }
            else
            {
                className = "ironUpTrans";
            }
            forgeUiVE.RemoveFromClassList(className);
            if (!stopAnim)
            {
                forgeUiVE.schedule.Execute(() =>
                {
                    forgeUiVE.AddToClassList(className);
                    black.style.visibility = Visibility.Hidden;
                }).StartingIn(50);
                forgeUiVE.schedule.Execute(() =>
                {
                    forgeUI.gameObject.SetActive(false);
                    upgradeUI.gameObject.SetActive(false);
                    gameManager.instance.SetPause(false);

                }).StartingIn(500);
            }
            else
            {
                forgeUI.gameObject.SetActive(false);
                upgradeUI.gameObject.SetActive(false);
                gameManager.instance.SetPause(false);
            }

                classActived = true;
        }
        else
        {
            gameManager.instance.SetPause(true);
            if (!Stats.Instance.ironTuto)
            {
                Tuto.Instance.LoadForgeTuto(true);
            }
            loadForgeUI();
        }

    }


    public void upIronRaffinedUi()
    {
        if (ironLabel != null)
        {
            ironLabel.text = Stats.Instance.iron.ToString();
        }

    }

    public override void loadForgeUI()
    {
        base.loadForgeUI();

        var root = forgeUI.rootVisualElement;
        uraniumButton = root.Q<Button>("uranium");
        prestigeButton = root.Q<Button>("prestige");
        ironLabel = root.Query<Label>("iron");
        forgeUiVE = root.Query<VisualElement>("forgeUI");
        if (!stopAnim)
        {
            if (classActived)
            {

                classActived = false;
                forgeUiVE.AddToClassList("forgeIronTrans");
            }

            forgeUiVE.schedule.Execute(() =>
            {
                forgeUiVE.RemoveFromClassList("forgeIronTrans");
            }).StartingIn(50);
        }

        ScrollView scroll = root.Q<ScrollView>("scroll");
        foreach (machineElement machine in Stats.Instance.machineIron)
        {
            scroll.Add(machine);
        }

        bool show = true;
        foreach (machineElement machine in Stats.Instance.machineIron)
        {
            if(show) { 
                machine.LoadMachine();
                machine.style.display = DisplayStyle.Flex;
            }
            else machine.style.display = DisplayStyle.None;

            if (!machine.isBuyed) show = false;
        }

        uraniumButton.clicked += uraniumClicked;
        prestigeButton.clicked += prestigeClicked;

    }

    public override void loadUpdateUI()
    {
        base.loadUpdateUI();
        var root = upgradeUI.rootVisualElement;
        uraniumButton = root.Q<Button>("uranium");
        prestigeButton = root.Q<Button>("prestige");
        ironLabel = root.Query<Label>("iron");
        forgeUiVE = root.Query<VisualElement>("updateUI");
        
        forgeUiVE.RemoveFromClassList("ironUpTrans"); 

        uraniumButton.clicked += uraniumClicked;
        prestigeButton.clicked += prestigeClicked;

        foreach (UpgradesIron up in Stats.Instance.upgradesIron)
        {
            up.loadUpgrade(upgradeUI);
        }
    }


    private void uraniumClicked()
    {

        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.uraniumUI.gameObject.SetActive(true);
        MainUi.Instance.uraniumUI.loadForgeUI();

    }

    private void prestigeClicked()
    {

        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.prestigeUI.forgeUI.gameObject.SetActive(true);
        MainUi.Instance.prestigeUI.loadForgeUI();

    }
}
