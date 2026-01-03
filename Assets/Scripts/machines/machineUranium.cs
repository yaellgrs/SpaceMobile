using UnityEngine;
using static UnityEngine.Android.AndroidGame;
using UnityEngine.UIElements;
using Unity.VisualScripting;

[System.Serializable]
public class machineUranium : Machine
{


    protected override void PayCost()
    {
        levelCostMachine1 = CalculUpgradeCost();
        Stats.Instance.upUranium(levelCostMachine1, false);
    }

    protected override void machine1Clicked()
    { 
        if (!isActive && Stats.Instance.uranium.isBigger(priceMachine))
        {
            contenerMachine1.style.display = DisplayStyle.None;
            if (machinePlus != null)
            {
                foreach (machineUranium m in Stats.Instance.machinesUranium)
                {
                    if (m.machineName == machinePlus)
                    {
                        m.isVisible = true;
                        m.machine1.style.display = DisplayStyle.Flex;
                    }
                }

            }

            isActive = true;
            updateScroll();
            if (MainUi.Instance.questUI.type == QuestUI.questType.upMachines)
            {
                MainUi.Instance.questUI.upQuest();
            }
        }
        else
        {
            if (machineTime1 < 0)
            {
                machineTime1 = 0f;
            }
        }
    }

    protected override void upMachine1Clicked()
    {
        if (isActive && Stats.Instance.uranium.isBigger(CalculUpgradeCost()) && borderColorMachine != borderColor.black)
        {
            base.upMachine1Clicked();

        }

    }

    public override void machineUpdate()
    {
        if (!CalculUpgradeCost().isBigger(Stats.Instance.uranium) && upButtonMachine1 != null)
        {
            upButtonMachine1.enabledSelf = true;
        }
        else if (upButtonMachine1 != null)
        {
            upButtonMachine1.enabledSelf = false;
        }
        upMachineCostText();

        if (!automatic)
        {

            if (isActive)
            {
                if (machineTime1 >= 0)
                {
                    machineTime1 += Time.deltaTime;
                    if (timeLabelMachine1 != null)
                    {
                        timeLabelMachine1.text = (machineTimeMaxReel - machineTime1).ToString("F2") + "s";
                        if (machineTime1 / machineTimeMaxReel > 0)
                        {
                            progressMachine.style.width = Length.Percent((machineTime1 / machineTimeMaxReel) * 100);
                        }
                    }
                    if (machineTime1 >= machineTimeMaxReel)
                    {
                        if (earnLabelMachine1 != null)
                        {
                            Stats.Instance.upUranium(machineEarn1, true);
                        }
                        machineTime1 = -1;
                        if (timeLabelMachine1 != null)
                        {
                            timeLabelMachine1.text = machineTimeMaxReel.ToString("F2") + "s";
                        }
                    }
                }
            }
        }
        else
        {
            if (isActive)
            {
                animAutoBar();
                machineTime1 += Time.deltaTime;
                BigNumber earnPerScd = new BigNumber(machineEarn1.Mantisse, machineEarn1.Exp);
                earnPerScd.Divide(machineTimeMaxReel);
                if (timeLabelMachine1 != null)
                {
                    timeLabelMachine1.text = "";
                    earnLabelMachine1.text = earnPerScd + " / s";
                    progressMachine.style.width = Length.Percent(100);
                }
                machineTime1 += Time.deltaTime;
                if (machineTime1 >= 1f)
                {
                    if (earnLabelMachine1 != null)
                    {
                        Stats.Instance.upUranium(earnPerScd, true);
                    }
                    machineTime1 = 0f;
                }

            }
        }
    }


    public override void upGap()
    {
        foreach (machineUranium m in Stats.Instance.machinesUranium)
        {
            if (m.machineNumber > machineNumber)
            {
                m.gap += 50;
                m.loadMachine(forgeUilink);
            }
        }
    }

    protected override void updateScroll()
    {
        ScrollView scroll = MainUi.Instance.uraniumUI.forgeUI.rootVisualElement.Q<ScrollView>("scroll");

        float savedScroll = scroll.verticalScroller.value;
        MainUi.Instance.uraniumUI.stopAnim = true;
        MainUi.Instance.uraniumUI.IronClicked();
        MainUi.Instance.uraniumUI.loadForgeUI();
        MainUi.Instance.uraniumUI.stopAnim = false;
        scroll = MainUi.Instance.uraniumUI.forgeUI.rootVisualElement.Q<ScrollView>("scroll");
        scroll.schedule.Execute(() => {
            scroll.verticalScroller.value = savedScroll;
        }).ExecuteLater(1);
    }
}


[System.Serializable]
public class UpgradesUranium : Upgrades
{
    public enum UpgradeType { SpeedAuto, AreaSlow, AreaWidth, RocketReload, RocketMultiplier }
    public UpgradeType upgradeType;
    
    protected override void loadStat()
    {
        switch (upgradeType)
        {
            case UpgradeType.SpeedAuto:
                statLabel.text = "Shoot/s : " + ((1f / (Stats.Instance.speedAuto) )).ToString("F2");
                break;
            case UpgradeType.AreaSlow:
                statLabel.text = "meteors speed : x" + (1f/Stats.Instance.areaSpeed).ToString("F2");
                break;
            case UpgradeType.AreaWidth:
                statLabel.text = "Area Width : " + Stats.Instance.areaSize.ToString("F2");
                break;
            case UpgradeType.RocketReload:
                statLabel.text = "Time to reload : " + Stats.Instance.rocketTimerMax.ToString("F2");
                break;
            case UpgradeType.RocketMultiplier:
                statLabel.text = "Damage : x" + Stats.Instance.rocketMultiplier.ToString("F2") ;
                break;
        }
    }

    protected override void PayCost()
    {
        levelCostMachine1 = CalculUpgradeCost();
        Stats.Instance.upUranium(levelCostMachine1, false);
    }
    protected override void upMachine1Clicked()
    {
        if (Stats.Instance.uranium.isBigger(CalculUpgradeCost()))
        {
            base.upMachine1Clicked();

            if (MainUi.Instance.questUI.type == QuestUI.questType.uraniumUpgrade)
            {
                MainUi.Instance.questUI.upQuest();
            }
        }
    }

    public override void update()
    {
        if (Stats.Instance.uranium.isBigger(CalculUpgradeCost()) && upButton != null)
        {
            upButton.enabledSelf = true;
        }
        else if (upButton != null)
        {
            upButton.enabledSelf = false;

        }

        upMachineCostText();
    }

    protected override void getReward()
    {

        switch (upgradeType)
        {
            case UpgradeType.SpeedAuto:
                Stats.Instance.speedAuto = 1f / (0.09f*(machineLevel1+1));
                Debug.Log("speed auto shoot : " + Stats.Instance.speedAuto);
                break;
            case UpgradeType.AreaSlow:
                Stats.Instance.areaSpeed = 1f + 0.5f*Mathf.Pow(machineLevel1 + 1, 0.6f);
                break;
            case UpgradeType.AreaWidth: 
                Stats.Instance.areaSize =1f+ 0.3f*Mathf.Pow(machineLevel1, 0.4f);
                spaceShip.instance.setAreaScale(Stats.Instance.areaSize);
                break;
            case UpgradeType.RocketReload:
                Stats.Instance.rocketTimerMax = 25f - Mathf.Pow(machineLevel1, 0.4f);
                break;
            case UpgradeType.RocketMultiplier:
                Stats.Instance.rocketMultiplier = 5f + 0.25f * Mathf.Pow(machineLevel1 - 1, 1.15f);
                break;

        }

        loadStat();
    }
}
