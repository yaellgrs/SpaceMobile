using System;
using System.IO;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable]
public class MachineIron : Machine
{       


    protected override void machine1Clicked()
    {
        if (!isActive && Stats.Instance.iron.isBigger(BN_price))
        {
            VE_container.style.display = DisplayStyle.None;
            if (nextMachine != null)
            {
                foreach (MachineIron m in Stats.Instance.machinesIron)
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
            if (MainUi.Instance.questUI.type == QuestUI.questType.unlockMachine)
            {
                MainUi.Instance.questUI.upQuest();
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

        if (isActive && Stats.Instance.iron.isBigger(CalculUpgradeCost()) && borderColorMachine != borderColor.black)
        {
            base.upMachine1Clicked();
        }
    }


    public override void machineUpdate()
    {
        if (Stats.Instance.iron.isBigger(CalculUpgradeCost()) && Btn_LevelUp != null)
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
                            Stats.Instance.upIron(BN_earn, true);
                        }
                        time = -1;



                        if (Lbl_time != null)
                        {
                            Lbl_time.text = timeMaxReal.ToString("F2") + "s";
                        }
                        if (!Stats.Instance.ironTuto)
                        {
                            Tuto.Instance.loadIronUpgradeTuto(true);

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
                        Stats.Instance.upIron(earnPerScd, true);
                    }
                    time = 0f;
                }

            }
        }

    }

    public override void upGap()
    {
        foreach (MachineIron m in Stats.Instance.machinesIron)
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
        ScrollView scroll = MainUi.Instance.ironUI.forgeUI.rootVisualElement.Q<ScrollView>("scroll");

        float savedScroll = scroll.verticalScroller.value;
        MainUi.Instance.ironUI.stopAnim = true;
        MainUi.Instance.ironUI.IronClicked();
        MainUi.Instance.ironUI.loadForgeUI();
        MainUi.Instance.ironUI.stopAnim = false;
        scroll = MainUi.Instance.ironUI.forgeUI.rootVisualElement.Q<ScrollView>("scroll");
        scroll.schedule.Execute(() => {
            Debug.Log("scroll value : " + savedScroll);
            scroll.verticalScroller.value = savedScroll;
        }).ExecuteLater(1); // attendre une frame pour que le layout soit prêt

        gameManager.instance.SetPause(true);
    }
}

[System.Serializable]
public class UpgradesIron : Upgrades
{
    public enum UpgradeType { Life, Damage, WorldSize, Shield, RegenShield }
    public UpgradeType upgradeType;

    protected override void loadStat()
    {
        switch (upgradeType)
        {
            case UpgradeType.Life:
                statLabel.text = "Life : " + Stats.Instance.lifeMax;
                break;
            case UpgradeType.Damage:
                statLabel.text = "Damage : " + spaceShip.instance.damage;
                break;
            case UpgradeType.WorldSize:
                statLabel.text = "WorldSize : " + ((200f / Stats.Instance.scale)-199f).ToString("F1");
                break;
            case UpgradeType.Shield:
                statLabel.text = "Shield : " + Stats.Instance.shieldMax;
                break;
            case UpgradeType.RegenShield:
                statLabel.text = "Regen Shield : " + Stats.Instance.regenShield;
                break;
            

        }
    }

    protected override void PayCost()
    {
        levelCostMachine1 = CalculUpgradeCost();
        Stats.Instance.upIron(levelCostMachine1, false);
    }

    protected override void upMachine1Clicked()
    {
        if (Stats.Instance.iron.isBigger(CalculUpgradeCost()))
        {
            base.upMachine1Clicked();
            if (MainUi.Instance.questUI.type == QuestUI.questType.ironUpgrade)
            {
                MainUi.Instance.questUI.upQuest();
            }

            if (!Stats.Instance.ironTuto)
            {
                Tuto.Instance.ironCloseTuto(true);
            }
        }
    }

    public override void update()
    {

        if (Stats.Instance.iron.isBigger(CalculUpgradeCost()) && upButton != null)
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
            case UpgradeType.Life:
                BigNumber diff = new BigNumber(spaceShip.instance.getMaxLife());
                diff.Subtract(Stats.Instance.life);

                Stats.Instance.lifeMax = new BigNumber(10, 0);
                Stats.Instance.lifeMax.Multiply(0.5f * Mathf.Pow(machineLevel1 + 1, 1.6f));

                Stats.Instance.life = new BigNumber(spaceShip.instance.getMaxLife());
                Stats.Instance.life.Subtract(diff);
                MainUi.Instance.upHealthBar();
                break;
            case UpgradeType.Damage:
                    spaceShip.instance.damage = new BigNumber(1, 0);
                    spaceShip.instance.damage.Multiply(Mathf.Pow( 1.3f, machineLevel1 ));
                    spaceShip.instance.damage.Add(0.5f*(machineLevel1-1));
                    break;
            case UpgradeType.WorldSize:
                Stats.Instance.scale = 1f;
                Stats.Instance.scale = Mathf.Pow(0.992f, machineLevel1 + 1);
                spaceShip.instance.setScale(Stats.Instance.scale);
                gameManager.instance.setMeteorScale();
                break;
            case UpgradeType.Shield:
                diff = new BigNumber(spaceShip.instance.getMaxShield());
                diff.Subtract(Stats.Instance.shield);
                Stats.Instance.shieldMax = new BigNumber(10, 0);
                Stats.Instance.shieldMax.Multiply(0.5f * Mathf.Pow(machineLevel1 + 1, 1.4f));
                Stats.Instance.shield = new BigNumber(spaceShip.instance.getMaxShield());
                Stats.Instance.shield.Subtract(diff);

                break;
            case UpgradeType.RegenShield:
                Stats.Instance.regenShield = new BigNumber(10, 0);
                Stats.Instance.regenShield.Multiply(0.20f * Mathf.Pow(machineLevel1 + 1, 1.30f));
                break;
            default:
                break;
        }

        loadStat();
    }



}
