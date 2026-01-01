using System;
using System.IO;
using System.Net.Http.Headers;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Machine
{
    public enum borderColor { white, bronze, silver, gold, diamand, black };

    public int machineNumber = 0;
    
    public Button machine1;
    protected Button upButtonMachine1;
    protected VisualElement progressMachine;
    protected VisualElement contenerMachine1;
    protected VisualElement cadre;
    protected VisualElement locked;
    protected Label levelLabelMachine1;
    protected Label earnLabelMachine1;
    protected Label timeLabelMachine1;
    protected Label levelCostLabelMachine1;
    protected Label priceMachineLabel;
    protected Label lockedLevel;

    public bool isActive = true;
    public bool isVisible = true;
    public string machineName;
    public float machineTimeMax1 = 3f;
    public float machineTimeMaxReel = 3f;

    public float machineTime1 = -1f;
    public int machineLevel1 = 1;
    public int realMachineLevel1 = 1;

    public int machineLevelMax = 5;
    public int machineLevelLimite = 5;

    public BigNumber priceMachine = new BigNumber(2000, 0);
    public BigNumber levelCostMachine1 = new BigNumber(10, 0);
    public BigNumber initialLevelCostMachine1 = new BigNumber(10, 0);
    public BigNumber machineEarn1 = new BigNumber(1, 0);
    public BigNumber initialEarnCostMachine1 = new BigNumber(1, 0);

    public string machinePlus = null;

    public bool automatic;
    public bool isBlackBorder = false;
    //DateTime datastart;
    public borderColor borderColorMachine = borderColor.white;
    protected int multiplicator;

    //auto bar
    protected int cptAuto = 1;
    public int gap = 0;
    protected float timerAuto = 0f;

    protected UIDocument forgeUilink;


    public virtual void loadMachine(UIDocument forgeUI)
    {
        forgeUilink = forgeUI;
        machine1 = forgeUI.rootVisualElement.Q<Button>(machineName);
        cadre = forgeUI.rootVisualElement.Q<Button>(machineName);

        if(isBlackBorder)
        {
            cadre.style.display = DisplayStyle.None;
            
            machine1 = forgeUI.rootVisualElement.Q<Button>(machineName + "bis");
            cadre = forgeUI.rootVisualElement.Q<Button>(machineName + "bis");
        }
       
        cadre.style.visibility = Visibility.Visible;
        contenerMachine1 = machine1.Q<VisualElement>("contener");

        upButtonMachine1 = machine1.Q<Button>("up");
        progressMachine = machine1.Query<VisualElement>("progressMachine1");
        locked = machine1.Query<VisualElement>("locked");
        levelLabelMachine1 = machine1.Query<Label>("level");
        levelCostLabelMachine1 = machine1.Query<Label>("levelCost");
        timeLabelMachine1 = machine1.Query<Label>("time");
        earnLabelMachine1 = machine1.Query<Label>("earn");
        priceMachineLabel = machine1.Query<Label>("priceMachine");
        lockedLevel = machine1.Query<Label>("levelLocked");
        machineTimeMaxReel = machineTimeMax1 * Stats.Instance.machineTimeReducer;

        machineLevelLimite = Stats.Instance.level +1;
        if(machineLevelLimite > machineLevelMax)
        {
            machineLevelLimite = machineLevelMax;
        }

        if (earnLabelMachine1 != null)
        {
            earnLabelMachine1.text = machineEarn1.ToString();
            if(machineLevel1 == machineLevelMax)
            {
                levelLabelMachine1.text = "UP";
            }
            else
            {
                levelLabelMachine1.text = machineLevel1 + "/" + machineLevelMax;
            }
            

            machine1.clicked += machine1Clicked;
            if(priceMachineLabel != null)
            {
                priceMachineLabel.text = priceMachine.ToString();
            }
            cadre.style.visibility = Visibility.Visible;

            cadre.style.translate = new Translate(0, gap, 0);
        }
        

        upButtonMachine1.clicked += upMachine1Clicked;
        levelCostMachine1 = CalculUpgradeCost();
        CalculEarn();
        timeLabelMachine1.text = machineTimeMaxReel.ToString("F1") + "s";
        if (levelCostLabelMachine1 != null)
        {
            levelCostLabelMachine1.text = levelCostMachine1.ToString();
        }

       

        if (contenerMachine1 != null)
        {
            if (isActive)
            {
                contenerMachine1.style.display = DisplayStyle.None;
            }
            else
            {
                contenerMachine1.style.display = DisplayStyle.Flex;
            }
        }
        if (!isVisible && !isActive)
        {
            machine1.style.display = DisplayStyle.None;
        }
        if (isVisible)
        {
            machine1.style.display = DisplayStyle.Flex;
        }

        setBorderColor();
    }

    protected virtual void machine1Clicked()
    {


    }


    public void upMachineCostText()
    {
        multiplicator = UpMode.Instance.upModeMultiplicator;
        if (multiplicator > machineLevelLimite - machineLevel1)
        {
            multiplicator = machineLevelLimite - machineLevel1;
        }
        if (levelCostLabelMachine1 != null)
        {

            if (Stats.Instance.level < machineLevel1)
            {
                if (locked != null)
                {
                    locked.style.visibility = Visibility.Visible;
                    lockedLevel.text = (machineLevelLimite).ToString();
                }
            }
            else
            {
                if(locked != null)
                {
                    locked.style.visibility = Visibility.Hidden;
                }

            }
            BigNumber convertInit = new BigNumber(initialLevelCostMachine1.Mantisse, initialLevelCostMachine1.Exp);
            convertInit = CalculUpgradeCost();
            levelCostLabelMachine1.text = convertInit.ToString();
            levelCostLabelMachine1.style.visibility = Visibility.Visible;
        }
    }

    protected virtual BigNumber CalculUpgradeCost()
    {
        BigNumber calculedNumber = new BigNumber(1, 0);
        BigNumber convertInit;
        float currentPow = Mathf.Pow(1.60f, realMachineLevel1); // début de la suite
        for (int i = 0; i < multiplicator; i++)
        {
            convertInit = new BigNumber(initialLevelCostMachine1.Mantisse, initialLevelCostMachine1.Exp);
            convertInit.Multiply(Stats.Instance.upgradesPriceReducer * currentPow);
            calculedNumber.Add(convertInit);
            currentPow = 1.6f;
        }
        if(multiplicator == 0)
        {
            convertInit = new BigNumber(initialLevelCostMachine1.Mantisse, initialLevelCostMachine1.Exp);
            convertInit.Multiply(3* Stats.Instance.upgradesPriceReducer * Mathf.Pow(1.50f, realMachineLevel1));
            calculedNumber.Add(convertInit);
        }

        calculedNumber.Normalize();
        return calculedNumber;
    }

    protected virtual void CalculEarn()
    {

        machineEarn1 = new BigNumber(initialEarnCostMachine1.Mantisse, initialEarnCostMachine1.Exp);
        machineEarn1.Multiply(Mathf.Pow(1.20f, realMachineLevel1));
        machineEarn1.Add(realMachineLevel1 - 1);
        Debug.Log("real machine level 1 :" + realMachineLevel1);
    }

    protected virtual void PayCost()
    {
        levelCostMachine1 = CalculUpgradeCost();
        Stats.Instance.upIron(levelCostMachine1, false);
    }

    protected virtual void upMachine1Clicked()
    {

        if (machineLevel1 < machineLevelLimite)
        {
            PayCost();
            multiplicator = UpMode.Instance.upModeMultiplicator;
            if (multiplicator > machineLevelLimite - machineLevel1)
            {
                multiplicator = machineLevelLimite - machineLevel1;
                if (multiplicator == 0)
                {
                    multiplicator++;
                }
            }
            machineLevel1 += multiplicator;
            realMachineLevel1 += multiplicator;


            CalculEarn();

            levelLabelMachine1.text = machineLevel1 + "/" + machineLevelMax;
            earnLabelMachine1.text = machineEarn1.ToString();
            timeLabelMachine1.text = machineTimeMaxReel.ToString("F1") + "s";
            levelCostLabelMachine1.text = levelCostMachine1.ToString();

            if (machineLevel1 == machineLevelLimite)
            {
                progressMachine.style.width = Length.Percent(100);
            }

            if(MainUi.Instance.questUI.type == QuestUI.questType.upMachines)
            {
                MainUi.Instance.questUI.upQuest();
            }
        }
        else if (machineLevel1 >= machineLevelMax)
        {
            PayCost();
            borderColorMachine++;
            setBorderColor();
            if (borderColorMachine != borderColor.black)
            {

                machineLevel1 = 1;

                machineTimeMax1 -= machineTimeMax1 / 3;
                machineTimeMaxReel -= machineTimeMaxReel / 3;
                timeLabelMachine1.text = machineTimeMaxReel.ToString("F1") + "s";
                levelLabelMachine1.text = "1/" + machineLevelMax;
                progressMachine.style.width = Length.Percent(100);
            }
            else
            {
                machineTimeMax1 -= machineTimeMax1 / 3;
                machineTimeMaxReel -= machineTimeMaxReel / 3;
                timeLabelMachine1.text = machineTimeMaxReel.ToString("F1") + "s";
                if(levelLabelMachine1 != null)
                {
                    levelLabelMachine1.text = "max";
                }

            }
        }
        
    }

    public virtual void machineUpdate()
    {
        if (!automatic)
        {
            upMachineCostText();
            if (isActive)
            {
                if (machineTime1 >= 0)
                {
                    machineTime1 += Time.deltaTime;
                    if (timeLabelMachine1 != null)
                    {
                        timeLabelMachine1.text = (machineTimeMaxReel - machineTime1).ToString("F1") + "s";
                        if (machineTime1 / machineTimeMaxReel > 0)
                        {
                            progressMachine.style.width = Length.Percent((machineTime1 / machineTimeMaxReel) * 100);
                        }
                    }

                    if (machineTime1 >= machineTimeMaxReel)
                    {
                        if (earnLabelMachine1 != null)
                        {
                            Stats.Instance.upIron(machineEarn1, true);
                        }
                        machineTime1 = -1;
                        if (timeLabelMachine1 != null)
                        {
                            timeLabelMachine1.text = machineTimeMaxReel.ToString("F1") + "s";
                            
                        }

                    }
                }

            }
        }
        else
        {
            upMachineCostText();
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

                if (earnLabelMachine1 != null)
                {
                    Stats.Instance.upIron(earnPerScd, true);
                }
                machineTime1 = 0f;
                

            }
        }
        
    }

    protected void animAutoBar()
    {
        timerAuto+= Time.deltaTime;
        string path = "bar/barAnim" + cptAuto;
        Texture2D texture = Resources.Load<Texture2D>(path);
        if(texture != null && progressMachine != null)
        {
            progressMachine.style.backgroundImage = texture;
        }
        if (timerAuto > 0.08f)
        {
            timerAuto = 0f;
            cptAuto--;
            if(cptAuto == 0)
            {
                cptAuto = 4;
            }
        }
    }


    protected void setBorderColor()
    {
        string pathCadre = "machines";
        string pathButton = "machines";

        switch (borderColorMachine)
        {
            case borderColor.bronze:
                machineLevelMax = 10;
                pathCadre += "/bronze/cadre";
                pathButton += "/bronze/button";
                break;
            case borderColor.silver:
                machineLevelMax = 25;
                pathCadre += "/iron/cadre";
                pathButton += "/iron/button";
                break;
            case borderColor.gold:
                machineLevelMax = 50;
                pathCadre += "/gold/cadre";
                pathButton += "/gold/button";
                break;
            case borderColor.diamand:
                machineLevelMax = 100;
                pathCadre += "/Diamand/cadre";
                pathButton += "/Diamand/button";
                break;
            case borderColor.black:
                setBlackBorder();
                break;

        }
        Texture2D textureCadre = Resources.Load<Texture2D>(pathCadre);
        Texture2D textureButton = Resources.Load<Texture2D>(pathButton);
        if (textureCadre != null)
        {
            upButtonMachine1.style.backgroundImage = new StyleBackground(textureCadre);
            upButtonMachine1.style.backgroundSize = new BackgroundSize(Length.Percent(101.5f), Length.Percent(74.5f));
            upButtonMachine1.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            upButtonMachine1.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        }


        if (textureCadre != null)
        {
            cadre.style.backgroundImage = textureCadre;
            upButtonMachine1.style.backgroundImage = textureButton;
        }
        else
        { 
        }
        machineLevelLimite = Stats.Instance.level +1;
        if (machineLevelLimite > machineLevelMax)
        {
            machineLevelLimite = machineLevelMax;
        }
    }

    private void setBlackBorder()
    {
        if (!isBlackBorder)
        {
            isBlackBorder = true;
            loadMachine(forgeUilink);
            upGap();
            updateScroll(); 
        }
    }

    public virtual void upGap()
    {
    }

    protected virtual void updateScroll()
    {

    }

}




[System.Serializable]
public class Upgrades
{

    protected VisualElement upgrade;
    public VisualElement cadre;
    public VisualElement locked;
    public string upgradeName;

    protected bool isUpgrade = false;
    protected Label statLabel;
    protected Label name;
    protected Label levelPlusLabelMachine1;
    protected Label levelLabel;
    protected Label levelCostLabel;
    protected Label levelLocked;
    protected Button upButton;

    public float machineTimeMax1 = 1f;
    public BigNumber levelCostMachine1 = new BigNumber(10, 0);
    public BigNumber initialLevelCostMachine1 = new BigNumber(100, 0);
    public int machineLevelMax1 = 15;
    public int machineLevel1 = 1;
    protected int multiplicator;





    public void loadUpgrade(UIDocument forgeUI)
    {
        upgrade = forgeUI.rootVisualElement.Q<VisualElement>(upgradeName);
        locked = upgrade.Q<VisualElement>("locked");
        statLabel = upgrade.Q<Label>("stat");
        levelCostLabel = upgrade.Q<Label>("levelCost");
        name = upgrade.Q<Label>("name");
        upButton = upgrade.Q<Button>("up");
        levelPlusLabelMachine1 = upgrade.Q<Label>("levelUp");
        levelLocked = upgrade.Q<Label>("levelLocked");
        levelLabel = upgrade.Q<Label>("level");
        cadre = forgeUI.rootVisualElement.Q<Button>(upgradeName);
        //levelPlusLabelMachine1.text = "Lv " +( machineLevel1 + 1 );
        loadStat();
        getReward();
        levelCostMachine1 = CalculUpgradeCost();
        levelCostLabel.text = levelCostMachine1.ToString(); 
        upButton.clicked += upMachine1Clicked;
        cadre.style.visibility = Visibility.Visible;
    }

    public void upMachineCostText()
    {
        multiplicator = UpMode.Instance.upModeMultiplicator;
        if (multiplicator > machineLevelMax1 - machineLevel1)
        {
            multiplicator = machineLevelMax1 - machineLevel1;
        }

        if (levelCostLabel != null)
        {

            if (machineLevelMax1 <= machineLevel1 || machineLevel1 >= Stats.Instance.level +1)
            {
                if (locked != null)
                {
                    locked.style.visibility = Visibility.Visible;
                    
                    if(machineLevel1 == machineLevelMax1)
                    {
                        levelLocked.text = "MAX";
                    }
                    else levelLocked.text = (Stats.Instance.level + 2).ToString();

                    upButton.SetEnabled(false);
                }
            }

            else
            {
                if (locked != null)
                {
                    locked.style.visibility = Visibility.Hidden;
                }
            }
            BigNumber convertInit = new BigNumber(initialLevelCostMachine1.Mantisse, initialLevelCostMachine1.Exp);
            convertInit = CalculUpgradeCost();
            levelCostLabel.text = convertInit.ToString();
            //levelPlusLabelMachine1.text = "Lv " + (machineLevel1 + multiplicator).ToString();
            if (levelLabel != null)
            {
                levelLabel.text = machineLevel1 + "/" + machineLevelMax1;
            }

        }


    }

    protected virtual BigNumber CalculUpgradeCost()
    {
        BigNumber calculedNumber = new BigNumber(1, 0);
        BigNumber convertInit;

        float currentPow = Mathf.Pow(1.60f, machineLevel1); // début de la suite
        if (multiplicator == 0)
        {
            multiplicator++;
        }
        for (int i = 0; i < multiplicator; i++)
        {
            convertInit = new BigNumber(initialLevelCostMachine1.Mantisse, initialLevelCostMachine1.Exp);
            convertInit.Multiply(currentPow);
            calculedNumber.Add(convertInit);
            currentPow *= 1.6f;
        }

        calculedNumber.Normalize();
        return calculedNumber;
    }

    protected virtual void PayCost()
    {
        levelCostMachine1 = CalculUpgradeCost();
        Stats.Instance.upUranium(levelCostMachine1, false);
    }

    protected virtual void loadStat()
    {

    }

    protected virtual void upMachine1Clicked()
    {
        if(machineLevel1 < machineLevelMax1)
        {
            PayCost();
            multiplicator = UpMode.Instance.upModeMultiplicator;

            if(machineLevel1 + multiplicator >= machineLevelMax1)
            {
                machineLevel1 = machineLevelMax1;
            }
            else machineLevel1 += multiplicator;

            getReward();

            if (machineLevel1 == machineLevelMax1)
            {
                levelCostLabel.text = "";
                levelPlusLabelMachine1.text = "Max";
            }
            else
            {
                levelCostLabel.text = levelCostMachine1.ToString();
                //levelPlusLabelMachine1.text = "Lv " + (machineLevel1 + 1).ToString();
            }
        }
    }
    public virtual void update()
    {

    }

    protected virtual void getReward()
    {
        
    }

}

