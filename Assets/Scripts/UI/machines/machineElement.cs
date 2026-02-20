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

    //variables
    private int levelMax = 5;
    private int levelLimite = 1;
    private int realLevel = 1;
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

        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, levelLimite - level);

        Lbl_buyPrice.text = BN_price.ToString();
        Lbl_name.text = machineName;

        SetBorderColor();
        upMachineCostText();
        LoadMachineInfos();

        clicked -= StartProduction;
        clicked += StartProduction;
        Btn_up.clicked -= LevelUp;
        Btn_up.clicked += LevelUp;

        SetLevelUpButton();
    }

    public void LoadMachineInfos()
    {
        Lbl_reward.text = "Reward : " + CalculReward().ToString();
        Lbl_employee.text = "Employee : " + (production_cps);
        Lbl_level.text = (level == levelMax) ? "Lv : UP" : "Lv : " + level + "/" + levelMax + " (+ " + getMulitplicator() + ")";
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
        if ((!canBuy(CalculLevelUpCost()) || !havelevel() ) && color != borderColor.black) return;

        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator,( levelLimite - level) + 1 );
        HandleMoney(-CalculLevelUpCost());
        level += multiplicator;
        realLevel += multiplicator;

        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, levelLimite - level);

        if (level > levelMax)
        {
            color++;
            SetBorderColor();
            if (color != borderColor.black)
            {
                level = 1;
            }
            else
            {
                Lbl_upCost.style.display = DisplayStyle.None;
            }
        }
        else
        {
            Lbl_upCost.text = CalculLevelUpCost().ToString();
        }

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
        float n = realLevel;
        BigNumber calculedNumber = new BigNumber(0);

        int mult = getMulitplicator();

        if (level == levelMax)//changement de grade ( ex : fer -> or )
        {
            calculedNumber.Set(BN_price);
            calculedNumber *= 3f * Mathf.Pow(1.75f, n);
            calculedNumber *= Stats.Instance.upgradesPriceReducer;
        }
        else
        {
            BigNumber temp = new BigNumber(0);
            double pow = Mathf.Pow(1.75f, n);
            for (int i = 0; i < mult; i++)
            {
                temp.Set(BN_price);
                temp.Multiply(pow, false);
                pow *= 1.75f;
                temp.Multiply(Stats.Instance.upgradesPriceReducer, false);
                calculedNumber.Add(temp, false);
            }
        }

        calculedNumber.Normalize();
        return calculedNumber;
    }
    /*
                 for (int i = 0; i < mult; i++)
            {
                temp.Set(BN_price);
                temp.Multiply(Mathf.Pow(1.75f, n + i));
                temp.Multiply(Stats.Instance.upgradesPriceReducer);
                calculedNumber.Add(temp);
            }
     
     
     */
    /*
 5 : bronze
 10 : argent
 25  : gold
50 : diamand 
100 : blackborder
 */

    public BigNumber CalculReward()
    {
        BigNumber reward = new BigNumber(1);
        reward.Multiply(Mathf.Pow(1.12f, realLevel)); //  1.2^reallevel * ( 0.5 * initialTIme^2 )
        reward.Add(realLevel - 1);
        reward *= BN_price *0.055f;
        return reward;
    }

    #endregion

    #region ------ set methods ------

    protected virtual void SetLevelUpButton()
    {
        Btn_up.enabledSelf = canBuy(CalculLevelUpCost());
    }

    public void upMachineCostText()
    {
        int limit = Ship.Current.level + 1;
        Lbl_lockedLevel.text = (limit).ToString();
        VE_lockedLevelCover.style.visibility = havelevel()? Visibility.Hidden : Visibility.Visible;
        Lbl_upCost.text = CalculLevelUpCost().ToString();
    }

    private bool havelevel()
    {
        int limit = Ship.Current.level + 1;
        return (level < limit);
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

        
        Color bronze = new Color(208 / 255.0f, 144 / 255.0f, 95 / 255.0f);
        Color silver = new Color(130 / 255.0f, 130 / 255.0f, 130 / 255.0f);
        Color gold = new Color(201 / 255.0f, 152 / 255.0f, 44 / 255.0f);
        Color diamand = new Color(2 / 255.0f, 208 / 255.0f, 202 / 255.0f);
        Color[] colors = { Color.white, bronze, silver, gold, diamand, Color.white };
        style.unityBackgroundImageTintColor = colors[(int)color];
        Btn_up.style.unityBackgroundImageTintColor = colors[(int)color];
        VE_logo.style.unityBackgroundImageTintColor = colors[(int)color];

        int[] levelMaxs = { 5, 10, 25, 50, 100, 100 };
        int[] cps = { 0, 1, 2, 4, 6, 10 };
        production_cps = cps[(int)color];
        levelMax = levelMaxs[(int)color];

        levelLimite = Mathf.Min(Ship.Current.level + 1, levelMax);
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
