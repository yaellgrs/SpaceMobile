using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
    [JsonIgnore] public Label Lbl_employee;
    [JsonIgnore] public Label Lbl_reward;
    [JsonIgnore] public Label Lbl_name;
    [JsonIgnore] public VisualElement VE_logo;

    #endregion

    #region ------ variables ------

    //constantes
    private static readonly int[] levelColor = { 5, 10, 25, 50, 100, 110 };
    private static readonly int[] cps = { 0, 1, 2, 4, 6, 10 };

    private static readonly Color bronze = new Color(208 / 255.0f, 144 / 255.0f, 95 / 255.0f);
    private static readonly Color silver = new Color(130 / 255.0f, 130 / 255.0f, 130 / 255.0f);
    private static readonly Color gold = new Color(201 / 255.0f, 152 / 255.0f, 44 / 255.0f);
    private static readonly Color diamand = new Color(2 / 255.0f, 208 / 255.0f, 202 / 255.0f);

    //variables
    private int levelMax = 100;
    private int nextColorlevel = 5;
    private int level = 1;

    private float time = 0f;

    BigNumber BN_price = new BigNumber(15000);

    public bool isBuyed = false;
    public bool isAutomatic = false;
    protected int multiplicator;

    public string machineName = "";

    borderColor color = borderColor.white;

    public int production_cps = 0;

    #endregion

    //methods
    #region ------ constructors ------

    public machineElement()
    {
        Init();
    }

    public machineElement(string machineName, BigNumber initPrice)
    {
        if(initPrice < new BigNumber(100))
            isBuyed = true;

        this.BN_price = initPrice;
        this.machineName = machineName;
        Init();
    }

    #endregion

    #region -------- INIT -------

    protected virtual void Init()
    {
        AddToClassList("machineCadre");//machineCadre
        AddToClassList("forgeButton");//machineCadrefdf

        Lbl_level = new Label();
        Lbl_employee = new Label();
        Lbl_reward = new Label();
        Lbl_name = new Label();
        VE_logo = new VisualElement();

        Lbl_level.text = "Lv : 1/5";
        Lbl_employee.text = "Employee : 0";
        Lbl_employee.name = "employee";
        Lbl_reward.text = "Reward : 350";
        Lbl_reward.name = "reward";
        Lbl_name.text = "Anvil";

        Lbl_name.name = "name";
        Lbl_level.name = "level";

        VE_logo.AddToClassList("machineLogo");
        Lbl_employee.AddToClassList("machineEmployee");
        Lbl_reward.AddToClassList("machineReward");
        Lbl_name.AddToClassList("machineName");
        Lbl_level.AddToClassList("machineLevel");

        Add(Lbl_level);
        Add(Lbl_employee);
        Add(Lbl_reward);
        Add(Lbl_name);
        Add(VE_logo);

        InitUpButton();
        InitBuyCover();

        SetLogos();
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
        VE_upCostLogo.style.backgroundImage = background;
        VE_buyLogo.style.backgroundImage = background;

        Lbl_upCost.AddToClassList(getLogoPath() + "Color");


        SetLogo();
    }
    #endregion

    #region ------ mainworkflow -------
    public virtual void LoadMachine()// a revoir
    {
        Lbl_upCost.text = CalculLevelUpCost().ToString();


        VE_buyCover.style.display = isBuyed ? DisplayStyle.None : DisplayStyle.Flex;

        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, getLimitLevel() - level);

        Lbl_buyPrice.text = BN_price.ToString();
        Debug.Log("price : " + BN_price.ToString() );
        Lbl_name.text = machineName;

        SetBorderColor();
        upMachineCostText();
        LoadMachineInfos();

        clicked -= StartProduction;
        clicked += StartProduction;
        Btn_up.clicked -= LevelUp;
        if(color != borderColor.black) Btn_up.clicked += LevelUp;

        SetLevelUpButton();
    }

    public void LoadMachineInfos()
    {
        BigNumber RewardInc = new BigNumber(CalculReward(level + getMulitplicator()));
        RewardInc.Subtract(CalculReward());
        Lbl_reward.text = $"Reward : {CalculReward().ToString()} <color=green>(+{RewardInc.ToString()})</color>";
        Lbl_employee.text = "Employee : " + (production_cps);
        Lbl_level.text = (level == levelMax) ? "Lv : UP" : $"Lv : {level}/{levelMax} <color=cyan>(+{getMulitplicator()})</color>";
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
        else if (isBuyed) 
        {
            HandleMoney(CalculReward());
            if (this is machineIronElement && !Stats.Instance.ironTuto)
                Tuto.Instance.AddMachineClicked();
        }
    }

    protected virtual void LevelUp()
    {
        Debug.Log("level up");
        if ((!canBuy(CalculLevelUpCost()) || !havelevel() ) || color == borderColor.black ) return;
        HandleMoney(-CalculLevelUpCost());
        level += multiplicator;

        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, getLimitLevel() - level);


        setMaxColor();
        Lbl_upCost.text = CalculLevelUpCost().ToString();

        if (QuestManager.Instance.type == QuestType.UpgradeMachine)
            QuestManager.Instance.upQuest();

        gameManager.instance.SmallVibrate();

        if (this is machineIronElement && !Stats.Instance.ironTuto)
        {
            Tuto.Instance.ironCloseTuto(true);
        }
        upMachineCostText();
        LoadMachineInfos();
    }

    public void setMaxColor()
    {
        int index = -1;
        for(int i = 0; i < levelColor.Length; i++)
        {
            if (levelColor[i] <= level)
                index = i;
            else
                break;
        }

        if(index != -1 && index != (int)color - 1 )
        {
            color = (borderColor)(index + 1);
            if (color == borderColor.black)
                Lbl_upCost.style.display = DisplayStyle.None;
            SetBorderColor();
        }
    }
    
    public virtual void Update()
    {
        if (!isBuyed) return;

        if (production_cps > 0) {
            time += Time.deltaTime;
            if (time >= (1.0f / (float)production_cps)){
                HandleMoney(CalculReward());
                time = 0f;
            }
        }
    }

    #endregion

    #region ------ calculs methods ------ 

    public int getMulitplicator()
    {
        return Mathf.Min(levelMax - level, UpMode.Instance.upModeMultiplicator);
    }

    protected BigNumber CalculLevelUpCost()
    {
        double r = 1.75;
        BigNumber calculedNumber = new BigNumber(0);

        int mult = getMulitplicator();

        if (level == nextColorlevel)//changement de grade ( ex : fer -> or )
        {
            calculedNumber.Set(BN_price);
            double factor = 3.00 * System.Math.Pow(r, level) * Stats.Instance.upgradesPriceReducer;
            calculedNumber.Multiply(factor, false);
        }
        else
        {
            double pow = System.Math.Pow(r, level);
            calculedNumber.Set(BN_price * Stats.Instance.upgradesPriceReducer);
            calculedNumber.Multiply(pow, false);
            double factor = (System.Math.Pow(r, mult) - 1) / (r - 1);
            calculedNumber.Multiply(factor, false);
            calculedNumber.Add(addColorCost(level, level + mult, r), false);
        }

        calculedNumber.Normalize();
        return calculedNumber;
    }

    private BigNumber addColorCost(int baseLevel, int endLevel, double r)
    {
        BigNumber addCost = new BigNumber(0);
        foreach(int lvColor in levelColor)
        {
            if (lvColor > baseLevel && lvColor <= endLevel)
            {
                BigNumber colorCost = new BigNumber(BN_price);
                double factor = 2.00 * System.Math.Pow(r, lvColor) * Stats.Instance.upgradesPriceReducer;
                colorCost.Multiply(factor, false);
                addCost.Add(colorCost, false);
            }
        }

        addCost.Normalize();
        return addCost;
    }

    public BigNumber CalculReward() { return CalculReward(level); }

    public BigNumber CalculReward(int lvl)
    {
        BigNumber reward = new BigNumber(1);
        reward.Multiply(Mathf.Pow(1.12f, lvl)); //  1.2^reallevel * ( 0.5 * initialTIme^2 )
        reward.Add(lvl - 1);

        reward *= BN_price *0.055f;
        Debug.Log("machine :  " + machineName + " reward: " + reward.ToString() + " price : " + BN_price.ToString() );
        return reward;
    }

    #endregion

    #region ------ set methods ------

    protected virtual void SetLevelUpButton()
    {
        Btn_up.enabledSelf = canBuy(CalculLevelUpCost()) || getRequireLevel(getMulitplicator()) > Ship.Current.level;
    }

    public void upMachineCostText()
    {
        Lbl_lockedLevel.text = (getRequireLevel(getMulitplicator())).ToString();
        VE_lockedLevelCover.style.visibility = havelevel(level + getMulitplicator())? Visibility.Hidden : Visibility.Visible;
        Lbl_upCost.text = CalculLevelUpCost().ToString();
    }

    private bool havelevel()
    {
        return havelevel(level);
    }

    private bool havelevel(int lv)
    {
        return (lv < getLimitLevel());
    }

    private int getLimitLevel()
    {
        int limit = (Ship.Current.level + 1) * 2;
        return Mathf.Min(100, limit);
    }
    private int getRequireLevel(int mult)
    {
        int targetLevel = level + mult;
        int requiredShipLevel = Mathf.CeilToInt(targetLevel / 2f) - 1;

        return Mathf.Max(0, requiredShipLevel);
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


            string pathCadre = "machines/" + color.ToString() + "/cadre";
            string pathButton = "machines/" + color.ToString() + "/button";

            Texture2D textureCadre = Resources.Load<Texture2D>(pathCadre);
            Texture2D textureButton = Resources.Load<Texture2D>(pathButton);

            if (textureCadre != null)
            {
                style.backgroundImage = textureCadre;
                Btn_up.style.backgroundImage = textureButton;
            }
        }
        else
        {
            styleSheets.Remove(blackBorderStyle);
            styleSheets.Add(styleSheet);
        }

        

        Color[] colors = { Color.white, bronze, silver, gold, diamand, Color.white };
        Debug.Log("color : " + color + "  index : " + (int)color);
        style.unityBackgroundImageTintColor = colors[(int)color];
        Btn_up.style.unityBackgroundImageTintColor = colors[(int)color];
        VE_logo.style.unityBackgroundImageTintColor = colors[(int)color];

        production_cps = cps[(int)color];
        nextColorlevel = levelColor[(int)color];
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

    #region ------ comparateurs ------
    public override bool Equals(object obj)
    {
        if (obj is machineElement other)
            return machineName == other.machineName;

        return false;
    }

    public override int GetHashCode()
    {
        return machineName.GetHashCode();
    }
#endregion
}
