using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public enum borderColor { white, bronze, iron, gold, diamand, black };

[UxmlElement]
public partial class machineElement : Button
{
    //attributs
    #region ------ UI Elements ------
    //progress Barre
    [JsonIgnore]  public VisualElement VE_progressCadre;
    [JsonIgnore] public VisualElement VE_rewardLogo;
    [JsonIgnore] public VisualElement VE_progressBar;
    [JsonIgnore] public Label Lbl_progressTime;
    [JsonIgnore] public Label Lbl_reward;
    [JsonIgnore] public Label Lbl_time;

    //Button
    [JsonIgnore] public Button Btn_up;
    [JsonIgnore] public VisualElement VE_lockedLevelCover;
    [JsonIgnore] public VisualElement VE_upCostLogo;
    [JsonIgnore] public Label Lbl_upName;
    [JsonIgnore] public Label Lbl_upCost;
    [JsonIgnore] public Label Lbl_lockedLevel;

    //buy
    [JsonIgnore] private Label Lbl_buyPrice;
    [JsonIgnore] private VisualElement VE_buyLogo;
    [JsonIgnore] private VisualElement VE_buyCover;

    //other
    [JsonIgnore] public Label Lbl_level;
    [JsonIgnore] public Label Lbl_name;
    [JsonIgnore] public VisualElement VE_logo;

    #endregion

    #region ------ variables ------

    //variables
    private float timeMax = 1f;
    public float timeMaxReal = 1f;
    private float initialTimeMax = 1f;

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
    public bool isAutomatic = false;
    protected int multiplicator;

    public string machineName = "";

    borderColor color = borderColor.white;

    #endregion

    //methods
    #region ------ constructors ------

    public machineElement()
    {
        Init();
    }

    public machineElement(string machineName, BigNumber initPrice, float time)
    {
        if(initPrice == new BigNumber(0))
            isBuyed = true;

        this.BN_price = initPrice;
        this.machineName = machineName;
        this.timeMax = this.initialTimeMax = time;
        Init();
    }

    #endregion

    #region -------- INIT -------

    protected virtual void Init()
    {
        AddToClassList("machineCadre");//machineCadre
        AddToClassList("forgeButton");//machineCadrefdf

        Lbl_level = new Label();
        Lbl_name = new Label();
        VE_logo = new VisualElement();

        Lbl_level.text = "1/5";
        Lbl_name.text = "Anvil";


        VE_logo.AddToClassList("machineLogo");
        Lbl_name.AddToClassList("machineName");
        Lbl_level.AddToClassList("machineLevel");

        VE_logo.Add(Lbl_level);
        Add(Lbl_name);
        Add(VE_logo);

        InitProgressBar();
        InitUpButton();
        InitBuyCover();

        SetLogos();

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

    public void SetLogos()
    {
        Texture2D logoTexture = Resources.Load<Texture2D>("logos/" + getLogoPath());
        StyleBackground background = new StyleBackground(logoTexture);
        VE_rewardLogo.style.backgroundImage = background;
        VE_upCostLogo.style.backgroundImage = background;
        VE_buyLogo.style.backgroundImage = background;

        Lbl_upCost.AddToClassList(getLogoPath() + "Color");
        Debug.LogWarning("add class : " + (getLogoPath() + "Color"));


        SetLogo();
    }
    #endregion

    #region ------ mainworkflow -------
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

        Lbl_buyPrice.text = BN_price.ToString();
        Lbl_name.text = machineName;

        SetBorderColor();
        SetEarnPerSecond();
        upMachineCostText();

        this.clicked += StartProduction;
        Btn_up.clicked -= LevelUp;
        Btn_up.clicked += LevelUp;

        SetLevelUpButton();
    }

    protected virtual void StartProduction() // == machine1Clicked
    {
        if (!isBuyed && canBuy(BN_price)) //buy machine
        {
            HandleMoney(-BN_price);
            VE_buyCover.style.display = DisplayStyle.None;
            isBuyed = true;
            if (QuestManager.Instance.type == QuestType.UnlockMachine)
            {
                QuestManager.Instance.upQuest();
            }
            reloadUI();
        }
        else if (time < 0)
        { //start production
            time = 0f;
        }
    }

    protected virtual void LevelUp()
    {
        if (!(canBuy(CalculLevelUpCost()) && color != borderColor.black)) return;

        Debug.Log("upMode : " + UpMode.Instance.upModeMultiplicator + " level limite : " +( levelLimite - level));

        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, levelLimite - level);
        HandleMoney(-CalculLevelUpCost());
        Debug.Log("level before " + level);
        level += multiplicator;
        realLevel += multiplicator;
        Debug.Log("level after " + level);
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

            if (color != borderColor.black)
            {
                level = 1;
                Lbl_level.text = "1/" + levelMax;
            }
            else
            {
                Lbl_level.text = "MAX";
                Lbl_upCost.style.display = DisplayStyle.None;
            }
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

        gameManager.instance.SmallVibrate();

        if (this is machineIronElement && !Stats.Instance.ironTuto)
        {
            Tuto.Instance.ironCloseTuto(true);
        }
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
                    HandleMoney(CalculReward());
                    time = -1f;
                    VE_progressBar.style.width = Length.Percent(100);
                    Lbl_time.text = timeMaxReal.ToString("F1") + "s";
                    if (this is machineIronElement && !Stats.Instance.ironTuto)
                        Tuto.Instance.AddMachineClicked();
                }
            }
        }
        else//automatic
        {
            time += Time.deltaTime;
            AnimAutoBar();
            if(Lbl_reward!= null ) Lbl_reward.text = BN_earnPerScd.ToString() + " / s";
            if(time >= 1f)
            {
                HandleMoney(BN_earnPerScd);
                time = 0f;
            }
        }

    }

    #endregion

    #region ------ calculs methods ------ 

    protected virtual BigNumber CalculLevelUpCost()
    {
        float n = realLevel;
        BigNumber calculedNumber = new BigNumber(0);

        if (multiplicator == 0)//changement de grade ( ex : fer -> or )
        {
            calculedNumber.Set(BN_price);
            calculedNumber *= 2.5f * Mathf.Pow(n, 1.7f);
            calculedNumber *= Stats.Instance.upgradesPriceReducer;
        }
        else
        {
            BigNumber temp = new BigNumber(0);
            for (int i = 0; i < multiplicator; i++)
            {
                temp.Set(BN_price);
                temp.Multiply(Mathf.Pow(n + i, 1.7f));
                temp.Multiply(Stats.Instance.upgradesPriceReducer);
                calculedNumber.Add(temp);
            }
        }

        calculedNumber.Normalize();
        return calculedNumber;
    }

    public BigNumber CalculReward()
    {
        BigNumber reward = new BigNumber(1);
        reward.Multiply(Mathf.Pow(1.20f, realLevel) * (initialTimeMax * initialTimeMax)); //  1.2^reallevel * ( 0.5 * initialTIme^2 )
        reward.Add(realLevel - 1);
        return reward;
    }

    #endregion

    #region ------ set methods ------
    private void SetEarnPerSecond()
    {
        BN_earnPerScd = new BigNumber(CalculReward());
        BN_earnPerScd.Divide(timeMaxReal);
        
    }

    protected virtual void SetLevelUpButton()
    {
        Btn_up.enabledSelf = canBuy(CalculLevelUpCost());
    }

    public void upMachineCostText()
    {
        Lbl_lockedLevel.text = (levelLimite).ToString();
        VE_lockedLevelCover.style.visibility = (Ship.Current.level < levelLimite) ? Visibility.Visible : VE_lockedLevelCover.style.visibility = Visibility.Hidden;

        Lbl_upCost.text = CalculLevelUpCost().ToString();
        //Lbl_upCost.style.visibility = Visibility.Visible; //utile ???
    }
    #endregion

    #region ------ adaptativeStyle ------
    protected void SetBorderColor()
    {
        StyleSheet blackBorderStyle = Resources.Load<StyleSheet>("styles/machineBlackBorderStyle");
        StyleSheet styleSheet = Resources.Load<StyleSheet>("styles/machineStyle");

        if (color == borderColor.black)
        {
            styleSheets.Add(blackBorderStyle);
            styleSheets.Remove(styleSheet);
        }
        else
        {
            styleSheets.Remove(blackBorderStyle);
            styleSheets.Add(styleSheet);
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

        int[] levelMaxs = { 5, 10, 25, 50, 100, 100 };
        levelMax = levelMaxs[(int)color];

        levelLimite = Mathf.Min(Ship.Current.level + 1, levelMax);
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

    #endregion

    #region ------ virtual methods ------

    protected virtual void HandleMoney(BigNumber amount)
    {

    }
    protected virtual bool canBuy(BigNumber price)
    {
        return false;
    }

    protected virtual void reloadUI()
    {

    }

    protected virtual string getLogoPath()
    {
        return "";
    }

    protected virtual void SetLogo()
    {

    }

    #endregion
}
