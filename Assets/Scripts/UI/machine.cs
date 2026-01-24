using System;
using System.IO;
using System.Net.Http.Headers;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

public class Machine
{
    public enum borderColor { white, bronze, iron, gold, diamand, black };

    public int machineNumber = 0;
    
    public Button Btn_machine;
    protected Button Btn_LevelUp;
    protected VisualElement VE_progressBarre;
    protected VisualElement VE_container;
    protected VisualElement VE_cadre;
    protected VisualElement VE_lockedCover;
    protected Label Lbl_level;
    protected Label Lbl_earn;
    protected Label Lbl_time;
    protected Label Lbl_cost;
    protected Label Lbl_price;
    protected Label Lbl_lockedLevel;

    public bool isActive = true;
    public bool isVisible = true;
    public string name;
    public float timeMax = 3f;
    public float timeMaxReal = 3f;

    public float time = -1f;
    public int level = 1;
    public int realLevel = 1;

    public int levelMax = 5;
    public int levelLimite = 5;

    public BigNumber BN_price = new BigNumber(2000, 0);
    public BigNumber BN_levelCost = new BigNumber(10, 0);
    public BigNumber BN_initLevelCost = new BigNumber(10, 0);
    public BigNumber BN_earn = new BigNumber(1, 0);
    public BigNumber BN_initEarn = new BigNumber(1, 0);

    public string nextMachine = null;

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
        Btn_machine = forgeUI.rootVisualElement.Q<Button>(name);
        VE_cadre = forgeUI.rootVisualElement.Q<Button>(name);

        if(isBlackBorder)
        {
            VE_cadre.style.display = DisplayStyle.None;
            
            Btn_machine = forgeUI.rootVisualElement.Q<Button>(name + "bis");
            VE_cadre = forgeUI.rootVisualElement.Q<Button>(name + "bis");
        }
       
        VE_cadre.style.visibility = Visibility.Visible;
        VE_container = Btn_machine.Q<VisualElement>("contener");

        Btn_LevelUp = Btn_machine.Q<Button>("up");
        VE_progressBarre = Btn_machine.Query<VisualElement>("progressMachine1");
        VE_lockedCover = Btn_machine.Query<VisualElement>("locked");
        Lbl_level = Btn_machine.Query<Label>("level");
        Lbl_cost = Btn_machine.Query<Label>("levelCost");
        Lbl_time = Btn_machine.Query<Label>("time");
        Lbl_earn = Btn_machine.Query<Label>("earn");
        Lbl_price = Btn_machine.Query<Label>("priceMachine");
        Lbl_lockedLevel = Btn_machine.Query<Label>("levelLocked");
        timeMaxReal = timeMax * Stats.Instance.machineTimeReducer;

        levelLimite = Stats.Instance.level +1;
        if(levelLimite > levelMax)
        {
            levelLimite = levelMax;
        }

        if (Lbl_earn != null)
        {
            Lbl_earn.text = BN_earn.ToString();
            if(level == levelMax)
            {
                Lbl_level.text = "UP";
            }
            else
            {
                Lbl_level.text = level + "/" + levelMax;
            }
            

            Btn_machine.clicked += machine1Clicked;
            if(Lbl_cost != null)
            {
                Lbl_cost.text = BN_price.ToString();
            }
            VE_cadre.style.visibility = Visibility.Visible;

            VE_cadre.style.translate = new Translate(0, gap, 0);
        }
        

        Btn_LevelUp.clicked += upMachine1Clicked;
        BN_levelCost = CalculUpgradeCost();
        CalculEarn();
        Lbl_time.text = timeMaxReal.ToString("F1") + "s";
        if (Lbl_cost != null)
        {
            Lbl_cost.text = BN_levelCost.ToString();
        }

       

        if (VE_container != null)
        {
            if (isActive)
            {
                VE_container.style.display = DisplayStyle.None;
            }
            else
            {
                VE_container.style.display = DisplayStyle.Flex;
            }
        }
        if (!isVisible && !isActive)
        {
            Btn_machine.style.display = DisplayStyle.None;
        }
        if (isVisible)
        {
            Btn_machine.style.display = DisplayStyle.Flex;
        }

        setBorderColor();
    }

    protected virtual void machine1Clicked()
    {


    }


    public void upMachineCostText()
    {
        multiplicator = UpMode.Instance.upModeMultiplicator;
        if (multiplicator > levelLimite - level)
        {
            multiplicator = levelLimite - level;
        }
        if (Lbl_cost != null)
        {

            if (Stats.Instance.level < level)
            {
                if (VE_lockedCover != null)
                {
                    VE_lockedCover.style.visibility = Visibility.Visible;
                    Lbl_lockedLevel.text = (levelLimite).ToString();
                }
            }
            else
            {
                if(VE_lockedCover != null)
                {
                    VE_lockedCover.style.visibility = Visibility.Hidden;
                }
            }
            BigNumber convertInit = new BigNumber(BN_initLevelCost.Mantisse, BN_initLevelCost.Exp);
            convertInit = CalculUpgradeCost();
            Lbl_cost.text = convertInit.ToString();
            Lbl_cost.style.visibility = Visibility.Visible;
        }
    }

    protected virtual BigNumber CalculUpgradeCost()
    { 
        float n = realLevel;
        BigNumber calculedNumber = new BigNumber(0);

        if (multiplicator == 0)//changement de grade ( ex : fer -> or )
        {
            calculedNumber = new BigNumber(BN_price.Mantisse, BN_price.Exp);
            calculedNumber.Multiply(5 * Mathf.Pow(n, 1.7f));
            calculedNumber.Multiply(Stats.Instance.upgradesPriceReducer);
        }
        else
        {
            for (int i = 0; i < multiplicator; i++)
            {
                BigNumber temp = new BigNumber(BN_price.Mantisse, BN_price.Exp);
                temp.Multiply(Mathf.Pow(n + i, 1.7f));
                temp.Multiply(Stats.Instance.upgradesPriceReducer);
                calculedNumber.Add(temp);
            }
        }

        calculedNumber.Normalize();
        return calculedNumber;
    }

    protected virtual void CalculEarn()
    {

        BN_earn = new BigNumber(BN_initEarn.Mantisse, BN_initEarn.Exp);
        BN_earn.Multiply(Mathf.Pow(1.20f, realLevel));
        BN_earn.Add(realLevel - 1);
    }

    protected virtual void PayCost()
    {
        BN_levelCost = CalculUpgradeCost();
        Stats.Instance.AddIron(-BN_levelCost);
    }

    protected virtual void upMachine1Clicked()
    {

        if (level < levelLimite)
        {
            PayCost();
            multiplicator = UpMode.Instance.upModeMultiplicator;
            if (multiplicator > levelLimite - level)
            {
                multiplicator = levelLimite - level;
                if (multiplicator == 0)
                {
                    multiplicator++;
                }
            }
            level += multiplicator;
            realLevel += multiplicator;


            CalculEarn();

            Lbl_level.text = level + "/" + levelMax;
            Lbl_earn.text = BN_earn.ToString();
            Lbl_time.text = timeMaxReal.ToString("F1") + "s";
            Lbl_cost.text = BN_levelCost.ToString();

            if (level == levelLimite)
            {
                VE_progressBarre.style.width = Length.Percent(100);
            }

            if(QuestManager.Instance.type == QuestType.UpgradeMachine)
            {
                QuestManager.Instance.upQuest();
            }
        }
        else if (level >= levelMax)
        {
            PayCost();
            borderColorMachine++;
            setBorderColor();
            if (borderColorMachine != borderColor.black)
            {

                level = 1;

                timeMax -= timeMax / 3;
                timeMaxReal -= timeMaxReal / 3;
                Lbl_time.text = timeMaxReal.ToString("F1") + "s";
                Lbl_level.text = "1/" + levelMax;
                VE_progressBarre.style.width = Length.Percent(100);
            }
            else
            {
                timeMax -= timeMax / 3;
                timeMaxReal -= timeMaxReal / 3;
                Lbl_time.text = timeMaxReal.ToString("F1") + "s";
                if(Lbl_level != null)
                {
                    Lbl_level.text = "max";
                }

            }
        }
        gameManager.instance.SmallVibrate();
    }

    public virtual void machineUpdate()
    {
        if (!automatic)
        {
            upMachineCostText();
            if (isActive)
            {
                if (time >= 0)
                {
                    time += Time.deltaTime;
                    if (Lbl_time != null)
                    {
                        Lbl_time.text = (timeMaxReal - time).ToString("F1") + "s";
                        if (time / timeMaxReal > 0)
                        {
                            VE_progressBarre.style.width = Length.Percent((time / timeMaxReal) * 100);
                        }
                    }

                    if (time >= timeMaxReal)
                    {
                        if (Lbl_earn != null)
                        {
                            Stats.Instance.AddIron(BN_earn);
                        }
                        time = -1;
                        if (Lbl_time != null)
                        {
                            Lbl_time.text = timeMaxReal.ToString("F1") + "s";
                            
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
                time += Time.deltaTime;
                BigNumber earnPerScd = new BigNumber(BN_earn.Mantisse, BN_earn.Exp);
                earnPerScd.Divide(timeMaxReal);
                if (Lbl_time != null)
                {
                    Lbl_time.text = "";
                    Lbl_earn.text = earnPerScd + " / s";
                    VE_progressBarre.style.width = Length.Percent(100);
                }

                if (Lbl_earn != null)
                {
                    Stats.Instance.AddIron(earnPerScd);
                }
                time = 0f;
                

            }
        }
        
    }

    protected void animAutoBar()
    {
        timerAuto+= Time.deltaTime;
        string path = "bar/barAnim" + cptAuto;
        Texture2D texture = Resources.Load<Texture2D>(path);
        if(texture != null && VE_progressBarre != null)
        {
            VE_progressBarre.style.backgroundImage = texture;
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
                levelMax = 10;
                pathCadre += "/bronze/cadre";
                pathButton += "/bronze/button";
                break;
            case borderColor.iron:
                levelMax = 25;
                pathCadre += "/iron/cadre";
                pathButton += "/iron/button";
                break;
            case borderColor.gold:
                levelMax = 50;
                pathCadre += "/gold/cadre";
                pathButton += "/gold/button";
                break;
            case borderColor.diamand:
                levelMax = 100;
                pathCadre += "/diamand/cadre";
                pathButton += "/diamand/button";
                break;
            case borderColor.black:
                setBlackBorder();
                break;

        }
        Texture2D textureCadre = Resources.Load<Texture2D>(pathCadre);
        Texture2D textureButton = Resources.Load<Texture2D>(pathButton);
        if (textureCadre != null)
        {
            Btn_LevelUp.style.backgroundImage = new StyleBackground(textureCadre);
            Btn_LevelUp.style.backgroundSize = new BackgroundSize(Length.Percent(101.5f), Length.Percent(74.5f));
            Btn_LevelUp.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            Btn_LevelUp.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        }


        if (textureCadre != null)
        {
            VE_cadre.style.backgroundImage = textureCadre;
            Btn_LevelUp.style.backgroundImage = textureButton;
        }
        else
        { 
        }
        levelLimite = Stats.Instance.level +1;
        if (levelLimite > levelMax)
        {
            levelLimite = levelMax;
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
    public VisualElement VE_logo;
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
        VE_logo = upgrade.Q<VisualElement>("logo");
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
        Stats.Instance.AddUranium(-levelCostMachine1);
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
        gameManager.instance.SmallVibrate();
    }
    public virtual void update()
    {

    }

    protected virtual void getReward()
    {
        
    }

}

