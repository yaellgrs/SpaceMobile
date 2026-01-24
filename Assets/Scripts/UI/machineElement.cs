using Newtonsoft.Json.Bson;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Machine;
using static UnityEngine.Android.AndroidGame;


/*
 Non pris en charge pour le moment : 
      - black border
      - afficher que le dernier non acheté ( juste ne pas mettre scroll.add(machine ) si isBuyed = false
      - gerer la mise en automatique
 */

[UxmlElement]
public partial class machineElement : Button
{
    //progress Barre
    public VisualElement VE_progressCadre;
    public VisualElement VE_rewardLogo;
    public VisualElement VE_progressBar;
    public Label Lbl_progressTime;
    public Label Lbl_reward;
    public Label Lbl_time;

    //Button
    public Button Btn_up;
    public VisualElement VE_lockedLevelCover;
    public VisualElement VE_upCostLogo;
    public Label Lbl_upName;
    public Label Lbl_upCost;
    public Label Lbl_lockedLevel;

    //buy
    private Label Lbl_buyPrice;
    private VisualElement VE_buyLogo;
    private VisualElement VE_buyCover;

    //other
    public Label Lbl_level;
    public Label Lbl_name;

    //variables
    private float timeMax = 1f;
    private float timeMaxReal = 1f;

    private int levelMax = 5;
    private int levelLimite = 1;
    private int realLevel = 1;
    private int level = 1;

    private float time = -1f;
    private float timerAuto = 0f;
    private int cptAuto = 1;

    BigNumber BN_earnPerScd;
    BigNumber BN_price = new BigNumber(15000);

    public bool isBuyed = false;
    private bool isAutomatic = false;
    protected int multiplicator;

    public string machineName = "";

    borderColor color = borderColor.white;

    public machineElement()
    {
        Init();
    }

    public machineElement(string machineName)
    {
        this.machineName = machineName;
        Init();
    }

    #region -------- INIT -------

    private void Init()
    {
        AddToClassList("machineCadre");//machineCadre
        AddToClassList("forgeButton");//machineCadre

        Lbl_level = new Label();
        Lbl_name = new Label();

        Lbl_level.text = "1/5";
        Lbl_name.text = "Anvil";
        Lbl_level.AddToClassList("machineLevel");
        Lbl_name.AddToClassList("machineName");

        Add(Lbl_level);
        Add(Lbl_name);

        InitProgressBar();
        InitUpButton();
        InitBuyCover();

        SetEarnPerSecond();
    }

    private void InitProgressBar()
    {
        VE_progressCadre = new VisualElement();
        VE_progressBar = new VisualElement();
        VE_rewardLogo = new VisualElement();

        Lbl_time = new Label();
        Lbl_progressTime = new Label();
        Lbl_reward = new Label();

        VE_progressCadre.AddToClassList("machineProgressCadre");
        VE_progressBar.AddToClassList("machineProgressBar");
        VE_rewardLogo.AddToClassList("machineRewardLogo");

        Lbl_time.text = "3s";
        Lbl_reward.text = "x10";
        Lbl_time.AddToClassList("machineTime");
        Lbl_reward.AddToClassList("machineReward");

        Add(VE_progressCadre);
        VE_progressCadre.Add(VE_progressBar);
        VE_progressCadre.Add(Lbl_time);
        VE_progressCadre.Add(Lbl_reward);
        Lbl_reward.Add(VE_rewardLogo);
    }

    private void InitUpButton()
    {
        Btn_up = new Button();
        VE_lockedLevelCover = new VisualElement();
        VE_upCostLogo = new VisualElement();
        Lbl_upName = new Label();
        Lbl_upCost = new Label();
        Lbl_lockedLevel = new Label();

        Btn_up.AddToClassList("machineUpButton");
        Btn_up.AddToClassList("button");

        VE_lockedLevelCover.AddToClassList("machineLockedCover");
        VE_upCostLogo.AddToClassList("machineUpCostLogo");

        Lbl_upName.text = "UPGRADE";
        Lbl_upCost.text = "10";
        Lbl_lockedLevel.text = "1";
        Lbl_upName.AddToClassList("machineUpName");
        Lbl_upCost.AddToClassList("machineUpCost");
        Lbl_lockedLevel.AddToClassList("machineLockedLevel");

        Add(Btn_up);
        Btn_up.Add(Lbl_upName);
        Btn_up.Add(Lbl_upCost);
        Btn_up.Add(VE_lockedLevelCover);

        Lbl_upCost.Add(VE_upCostLogo);
        VE_lockedLevelCover.Add(Lbl_lockedLevel);
    }

    private void InitBuyCover()
    {
        VE_buyCover = new VisualElement();
        VE_buyLogo = new VisualElement();
        Lbl_buyPrice = new Label();

        VE_buyCover.AddToClassList("machineBuyCover");
        VE_buyLogo.AddToClassList("machineBuyCoverLogo");
        Lbl_buyPrice.AddToClassList("machineBuyCoverPrice");

        Lbl_buyPrice.text = "15k";

        Add(VE_buyCover);
        VE_buyCover.Add(Lbl_buyPrice);
        Lbl_buyPrice.Add(VE_buyLogo);
    }
    #endregion


    public virtual void LoadMachine()// a revoir
    {
        timeMaxReal = timeMax * Stats.Instance.machineTimeReducer;

        Lbl_reward.text = CalculReward().ToString();//???
        Lbl_level.text = (level == levelMax) ? "UP" : Lbl_level.text = level + "/" + levelMax;
        Lbl_upCost.text = CalculLevelUpCost().ToString();

        if (isAutomatic)
        {
            Lbl_time.text = "";

            VE_progressBar.style.width = Length.Percent(100);
        }
        else
            Lbl_time.text = timeMaxReal.ToString("F1") + "s";

        VE_buyCover.style.display = isBuyed ? DisplayStyle.None : DisplayStyle.Flex;

        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, levelLimite - level);

        SetBorderColor();
        SetEarnPerSecond();
        SetLevelUpButton();

        this.clicked += StartProduction;
        Btn_up.clicked += LevelUp;
    }


    protected virtual void StartProduction() // == machine1Clicked
    {
        if (!isBuyed && Stats.Instance.iron.isBigger(BN_price))
        {
            VE_buyCover.style.display = DisplayStyle.None;
            isBuyed = true;
            if (QuestManager.Instance.type == QuestType.UnlockMachine)
            {
                QuestManager.Instance.upQuest();
            }
            MainUi.Instance.ironUI.loadForgeUI();
        }
        else if (time < 0)  time = 0f;
    }

    private void LevelUp()
    {
        if (!(Stats.Instance.iron.isBigger(CalculLevelUpCost()) && color != borderColor.black)) return;
        

        Stats.Instance.AddIron(-CalculLevelUpCost());

        level += multiplicator;
        realLevel += multiplicator;
        upMachineCostText();

        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, levelLimite - level);
        SetEarnPerSecond();

        if (level >= levelMax)
        {
            color++;
            SetBorderColor();

            timeMax -= timeMax / 3;
            timeMaxReal -= timeMaxReal / 3;
            Lbl_time.text = timeMaxReal.ToString("F1") + "s";
            timeMax -= timeMax / 3;
            timeMaxReal -= timeMaxReal / 3;
            Lbl_time.text = timeMaxReal.ToString("F1") + "s";

            if (color != borderColor.black)
            {
                level = 1;
                Lbl_level.text = "1/" + levelMax;
            }
            else
                Lbl_level.text = "max";
        }
        else
        {
            Lbl_level.text = level + "/" + levelMax;
            Lbl_reward.text = CalculReward().ToString();
            Lbl_time.text = timeMaxReal.ToString("F1") + "s";
            Lbl_upCost.text = CalculLevelUpCost().ToString();
        }

        if (QuestManager.Instance.type == QuestType.UpgradeMachine)
            QuestManager.Instance.upQuest();

        SetLevelUpButton();
        gameManager.instance.SmallVibrate();
    }

    
    public virtual void Update()
    {
        if (!isBuyed) return;

        if (!isAutomatic)
        {
            if (time >= 0)
            {
                time += Time.deltaTime;
                Lbl_time.text = (timeMaxReal - time).ToString("F1") + "s";
                if (time / timeMaxReal > 0)
                {
                    VE_progressBar.style.width = Length.Percent((time / timeMaxReal) * 100);
                }

                if (time >= timeMaxReal)
                {
                    Stats.Instance.AddIron(CalculReward());
                    time = -1f;
                    VE_progressBar.style.width = Length.Percent(100);
                    Lbl_time.text = timeMaxReal.ToString("F1") + "s";
                    SetLevelUpButton();
                }
            }
        }
        else
        {
            time += Time.deltaTime;
            AnimAutoBar();
            if(Lbl_reward!= null ) Lbl_reward.text = BN_earnPerScd.ToString() + " / s";
            if(time >= 1f)
            {
                Stats.Instance.AddIron(BN_earnPerScd);
                time = 0f;
                SetLevelUpButton();
            }
        }

    }



    public void upMachineCostText()
    {
        Lbl_lockedLevel.text = (levelLimite).ToString();
        VE_lockedLevelCover.style.visibility = (Stats.Instance.level < level)? Visibility.Visible : VE_lockedLevelCover.style.visibility = Visibility.Hidden; 

        Lbl_upCost.text = CalculLevelUpCost().ToString();
        //Lbl_upCost.style.visibility = Visibility.Visible; //utile ???
    }

    protected virtual BigNumber CalculLevelUpCost()
    {
        float n = realLevel;
        BigNumber calculedNumber = new BigNumber(0);

        if (multiplicator == 0)//changement de grade ( ex : fer -> or )
        {
            calculedNumber = new BigNumber(25);
            calculedNumber.Multiply(5 * Mathf.Pow(n, 1.7f));
            calculedNumber.Multiply(Stats.Instance.upgradesPriceReducer);
        }
        else
        {
            for (int i = 0; i < multiplicator; i++)
            {
                BigNumber temp = new BigNumber(25);
                temp.Multiply(Mathf.Pow(n + i, 1.7f));
                temp.Multiply(Stats.Instance.upgradesPriceReducer);
                calculedNumber.Add(temp);
            }
        }

        calculedNumber.Normalize();
        return calculedNumber;
    }

    private BigNumber CalculReward()
    {
        BigNumber reward = new BigNumber(1);
        reward.Multiply(Mathf.Pow(1.20f, realLevel));
        reward.Add(realLevel - 1);
        return reward;
    }


    protected void SetBorderColor()
    {
        if(color == borderColor.black)
        {
            SetBlackBorder();
            return;
        }

        string pathCadre = "machines/" + color.ToString() + "/cadre";
        string pathButton = "machines/" + color.ToString() + "/button";

        Texture2D textureCadre = Resources.Load<Texture2D>(pathCadre);
        Texture2D textureButton = Resources.Load<Texture2D>(pathButton);

        if (textureCadre != null)
        {
            this.style.backgroundImage = textureCadre;
            Btn_up.style.backgroundImage = textureButton;
        }

        int[] levelMaxs = { 5, 10, 25, 50, 100 };
        levelMax = levelMaxs[(int)color];

        levelLimite = Mathf.Min(Stats.Instance.level + 1, levelMax);
    }

    private void SetEarnPerSecond()
    {
        BN_earnPerScd = new BigNumber(CalculReward());
        BN_earnPerScd.Divide(timeMaxReal);
        
    }

    private void SetLevelUpButton()
    {
        Btn_up.enabledSelf = Stats.Instance.iron.isBigger(CalculLevelUpCost());
    }

    private void AnimAutoBar()
    {
        timerAuto += Time.deltaTime;
        string path = "bar/barAnim" + cptAuto;
        Texture2D texture = Resources.Load<Texture2D>(path);
        if (texture != null)
        {
            VE_progressBar.style.backgroundImage = texture;
        }
        else
        {
            Debug.LogWarning("texture auto null");
        }
        if (timerAuto > 0.08f)
        {
            timerAuto = 0f;
            cptAuto--;
            if (cptAuto == 0)
            {
                cptAuto = 4;
            }
        }
    }

    private void SetBlackBorder()
    {
        //a definir
    }

}
