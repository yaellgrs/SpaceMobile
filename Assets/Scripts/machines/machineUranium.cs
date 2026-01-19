using UnityEngine;
using static UnityEngine.Android.AndroidGame;
using UnityEngine.UIElements;
using Unity.VisualScripting;

[System.Serializable]
public class machineUranium : Machine
{


    protected override void PayCost()
    {
        BN_levelCost = CalculUpgradeCost();
        Stats.Instance.upUranium(BN_levelCost, false);
    }

    protected override void machine1Clicked()
    { 
        if (!isActive && Stats.Instance.uranium.isBigger(BN_price))
        {
            VE_container.style.display = DisplayStyle.None;
            if (nextMachine != null)
            {
                foreach (machineUranium m in Stats.Instance.machinesUranium)
                {
                    if (m.name == nextMachine)
                    {
                        m.isVisible = true;
                        m.Btn_machine.style.display = DisplayStyle.Flex;
                    }
                }

            }

            isActive = true;
            updateScroll();
            if (QuestManager.Instance.type == QuestType.UpgradeMachine)
            {
                QuestManager.Instance.upQuest();
            }
        }
        else
        {
            if (time < 0)
            {
                time = 0f;
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
        if (!CalculUpgradeCost().isBigger(Stats.Instance.uranium) && Btn_LevelUp != null)
        {
            Btn_LevelUp.enabledSelf = true;
        }
        else if (Btn_LevelUp != null)
        {
            Btn_LevelUp.enabledSelf = false;
        }
        upMachineCostText();

        if (!automatic)
        {

            if (isActive)
            {
                if (time >= 0)
                {
                    time += Time.deltaTime;
                    if (Lbl_time != null)
                    {
                        Lbl_time.text = (timeMaxReal - time).ToString("F2") + "s";
                        if (time / timeMaxReal > 0)
                        {
                            VE_progressBarre.style.width = Length.Percent((time / timeMaxReal) * 100);
                        }
                    }
                    if (time >= timeMaxReal)
                    {
                        if (Lbl_earn != null)
                        {
                            Stats.Instance.upUranium(BN_earn, true);
                        }
                        time = -1;
                        if (Lbl_time != null)
                        {
                            Lbl_time.text = timeMaxReal.ToString("F2") + "s";
                        }

                        if (!Stats.Instance.uraniumTuto)
                        {
                            Tuto.Instance.loadIronUpgradeTuto(false);

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
                time += Time.deltaTime;
                BigNumber earnPerScd = new BigNumber(BN_earn.Mantisse, BN_earn.Exp);
                earnPerScd.Divide(timeMaxReal);
                if (Lbl_time != null)
                {
                    Lbl_time.text = "";
                    Lbl_earn.text = earnPerScd + " / s";
                    VE_progressBarre.style.width = Length.Percent(100);
                }
                time += Time.deltaTime;
                if (time >= 1f)
                {
                    if (Lbl_earn != null)
                    {
                        Stats.Instance.upUranium(earnPerScd, true);
                    }
                    time = 0f;
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
        VisualElement logo = upgrade.Q<VisualElement>("logo");
        string logoPath = "Upgrades/uranium/";
        Texture2D logoTexutre = Resources.Load<Texture2D>(logoPath + upgradeType);
        if (logoTexutre == null) logoTexutre = Resources.Load<Texture2D>(logoPath + "CadresBlanc");
        logo.style.backgroundImage = logoTexutre;

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

            if (QuestManager.Instance.type == QuestType.UpgradeUranium)
            {
                QuestManager.Instance.upQuest();
            }
            if (!Stats.Instance.uraniumTuto)
            {
                Tuto.Instance.ironCloseTuto(false);
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
