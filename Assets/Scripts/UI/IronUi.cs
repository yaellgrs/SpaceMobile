using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Filters;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
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

    [SerializeField] public Sprite newSprite;
    public override void initializeMachine()
    {

        List<MachineIron> mach = new List<MachineIron>();
        MachineIron machine5 = new MachineIron()
        {
            machineName = "machine5",
            machineNumber = 5,
            isVisible = false,
            isActive = false,
            machineTimeMax1 = 1200f,
            levelCostMachine1 = new BigNumber(1, 10),
            initialLevelCostMachine1 = new BigNumber(1, 10),
            machineEarn1 = new BigNumber(1, 9),
            initialEarnCostMachine1 = new BigNumber(1, 9),
            priceMachine = new BigNumber(1,12),
            automatic = false
        };
        MachineIron machine4 = new MachineIron()
        {
            machinePlus = "machine5",
            machineName = "machine4",
            machineNumber = 4,
            isVisible = false,
            isActive = false,
            machineTimeMax1 = 300f,
            levelCostMachine1 = new BigNumber(5, 6),
            initialLevelCostMachine1 = new BigNumber(5, 6),
            machineEarn1 = new BigNumber(1,6),
            initialEarnCostMachine1 = new BigNumber(1, 6),
            priceMachine = new BigNumber(1, 9),
            automatic = false
        };
        MachineIron machine3 = new MachineIron()
        {
            machinePlus = "machine4",
            machineName = "machine3",
            machineNumber = 3, 
            isVisible = false,
            isActive = false,
            machineTimeMax1 = 60f,
            levelCostMachine1 = new BigNumber(20, 4),
            initialLevelCostMachine1 = new BigNumber(20, 4),
            machineEarn1 = new BigNumber(5,4),
            initialEarnCostMachine1 = new BigNumber(5, 4),
            priceMachine = new BigNumber(1, 6),
            automatic = false
        };

        MachineIron machine2 = new MachineIron()
        {
            machinePlus = "machine3",
            machineName = "machine2",
            machineNumber = 2, 
            isActive = false,
            isVisible = true,
            machineTimeMax1 = 15f,
            levelCostMachine1 = new BigNumber(2.5f, 3),
            initialLevelCostMachine1 = new BigNumber(2.5f,4),
            machineEarn1 = new BigNumber(5, 2),
            initialEarnCostMachine1 = new BigNumber(5, 2),
            priceMachine = new BigNumber(1, 3),
            automatic = false,
        };

        MachineIron machine1 = new MachineIron()
        {
            machinePlus = "machine2",
            machineName = "machine1",
            machineNumber = 1,
            isActive = true,
            isVisible = true,
            machineTimeMax1 = 1f,
            levelCostMachine1 = new BigNumber(25, 0),
            initialLevelCostMachine1 = new BigNumber(25, 0),
            machineEarn1 = new BigNumber(1, 0),
            initialEarnCostMachine1 = new BigNumber(1, 0),
            automatic = false
        };
        mach.Add(machine1);
        mach.Add(machine2);
        mach.Add(machine3);
        mach.Add(machine4);
        mach.Add(machine5);
        foreach (MachineIron m in mach)
        {
            int x = 0;
            foreach (MachineIron machUp in Stats.Instance.machinesIron)
            {
                if (m.machineName == machUp.machineName)
                {
                    x = 1;
                }
            }
            if (x == 0)
            {
                Stats.Instance.machinesIron.Add(m);
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

        foreach (Machine machine in Stats.Instance.machinesIron)
        {
            machine.machineUpdate();
        }
        foreach(Upgrades upgrade in Stats.Instance.upgradesIron)
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
                Tuto.Instance.loadIronForgeTuto();
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


        foreach (MachineIron machine in Stats.Instance.machinesIron)
        {
            machine.loadMachine(forgeUI);   
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
