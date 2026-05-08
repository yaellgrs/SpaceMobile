using GoogleMobileAds.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.Video;

public enum borderColor { white, bronze, iron, gold, diamand, black };

[Serializable]
public class machineData
{
    public machineData()
    {
        BN_price = new BigNumber(0);
        this.machineName = "default";
    }
    public machineData(string machineName, BigNumber initPrice)
    {
        if (initPrice <= new BigNumber(1000))
            isBuyed = true;

        BN_price = initPrice;
        this.machineName = machineName;
    }
    //constantes
    public static readonly int[] levelColor = { 1, 5, 10, 25, 50, 100, 110 };

    public Dictionary<borderColor, BorderBuyType> borderbuys = new Dictionary<borderColor, BorderBuyType>() {
        {borderColor.bronze, BorderBuyType.unbuyed},
        {borderColor.iron, BorderBuyType.unbuyed},
        {borderColor.gold, BorderBuyType.unbuyed},
        {borderColor.diamand, BorderBuyType.unbuyed},
        {borderColor.black, BorderBuyType.unbuyed},
    };


    //variables
    public int levelMax = 100;
    public int nextColorlevel = 5;
    public int level = 1;

    public float time = 0f;

    public BigNumber BN_price = new BigNumber(15000);

    public bool isBuyed = false;
    public int multiplicator;

    public string machineName = "";

    public borderColor color = borderColor.white;

    public int production_cps = 0;


    public override bool Equals(object obj)
    {
        if (obj is machineData other)
            return machineName == other.machineName;

        return false;
    }

    public override int GetHashCode()
    {
        return machineName.GetHashCode();
    }
}

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
    [JsonIgnore] protected Label Lbl_buyPrice;
    [JsonIgnore] private VisualElement VE_buyLogo;
    [JsonIgnore] private VisualElement VE_buyCover;

    //other
    [JsonIgnore] public Label Lbl_level;
    [JsonIgnore] public Label Lbl_employee;
    [JsonIgnore] public VisualElement VE_employeeLogo;
    [JsonIgnore] public Label Lbl_reward;
    [JsonIgnore] public Label Lbl_name;
    [JsonIgnore] public VisualElement VE_logo;

    [JsonIgnore] public Button Btn_LockBorderButtons;


    //parent

    #endregion

    #region ------ variables ------

    public machineData data;

    #endregion

    //methods
    #region ------ constructors ------

    public machineElement()
    {
        data = new machineData();
        Init();
    }

    public machineElement(machineData data)
    {
        this.data = data;
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
        VE_employeeLogo = new VisualElement();
        Lbl_reward = new Label();
        Lbl_name = new Label();
        VE_logo = new VisualElement();
        Btn_LockBorderButtons = new Button();

        Lbl_level.text = "Lv : 1/5";
        Lbl_employee.text = "x0";
        Lbl_employee.name = "employee";
        Lbl_reward.text = "Reward : 350";
        Lbl_reward.name = "reward";
        Lbl_name.text = "Anvil";

        Lbl_name.name = "name";
        Lbl_level.name = "level";

        VE_logo.AddToClassList("machineLogo");
        Lbl_employee.AddToClassList("machineEmployee");
        VE_employeeLogo.AddToClassList("machineEmployeeLogo");
        Lbl_reward.AddToClassList("machineReward");
        Lbl_name.AddToClassList("machineName");
        Lbl_level.AddToClassList("machineLevel");
        Btn_LockBorderButtons.AddToClassList("lockBorderButtons");
        Btn_LockBorderButtons.name = "lockBorderButtons";

        Add(Lbl_level);

        Add(VE_employeeLogo);
        VE_employeeLogo.Add(Lbl_employee);
        Add(Lbl_reward);
        Add(Lbl_name);
        Add(VE_logo);
        Add(Btn_LockBorderButtons);

        Btn_LockBorderButtons.clicked -= () => { BorderUI.Instance?.Open(this); };
        Btn_LockBorderButtons.clicked += () => { BorderUI.Instance?.Open(this); };





        InitUpButton();
        InitBuyCover();

        SetLogos();
        LoadLockBorders();
    }

    private void LoadLockBorders()
    {
        Btn_LockBorderButtons.style.display = data.level >= 5 ? DisplayStyle.Flex : DisplayStyle.None;
        Btn_LockBorderButtons.Clear();
        int i = 0;


        foreach (var val in data.borderbuys)
        {
            if (data.level < machineData.levelColor[(int)val.Key]) continue;
            LockBorderElement lockBorder = new LockBorderElement(this, val.Key, val.Value);
            Btn_LockBorderButtons.Add(lockBorder);
            i++;
        }
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
        Texture2D logoTexture = Resources.Load<Texture2D>( getLogoPath());

        StyleBackground background = new StyleBackground(logoTexture);
        VE_upCostLogo.style.backgroundImage = background;
        VE_buyLogo.style.backgroundImage = background;


        SetLogo();

        Lbl_upCost.style.color = getColor();
        Lbl_name.style.color = getColor();




    }
    #endregion

    #region ------ mainworkflow -------
    public virtual void LoadMachine()// a revoir
    {
        Lbl_upCost.text = CalculLevelUpCost().ToString();


        VE_buyCover.style.display = data.isBuyed ? DisplayStyle.None : DisplayStyle.Flex;

        data.multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, getLimitLevel() - data.level);

        Lbl_buyPrice.text = data.BN_price.ToString();
        Lbl_name.text = data.machineName;

        SetBorderColor();
        upMachineCostText();
        LoadMachineInfos();

        SetLogos();
        LoadLockBorders();

        clicked -= StartProduction;
        clicked += StartProduction;
        Btn_up.clicked -= LevelUp;
        if(data.color != borderColor.black) Btn_up.clicked += LevelUp;

        SetLevelUpButton();
    }

    public void LoadMachineInfos()
    {
        BigNumber RewardInc = new BigNumber(CalculReward(data.level + getMulitplicator()));
        RewardInc.Subtract(CalculReward());
        Lbl_reward.text = $"Reward : {CalculReward().ToString()} <color=green>(+{RewardInc.ToString()})</color>";
        Lbl_employee.text = "x" + (data.production_cps).ToString();
        //Lbl_employee.text += (GetColorAmount() > 0 )? $"<color=green>(+{GetColorAmount().ToString()})</color>" : "";
        Lbl_level.text = (data.level == data.levelMax) ? "Lv : UP" : $"Lv : {data.level}/{data.levelMax} <color=cyan>(+{getMulitplicator()})</color>";
    }

    protected virtual void StartProduction() // == machine1Clicked
    {
        if (!data.isBuyed && canBuy(data.BN_price)) //buy machine
        {
            HandleMoney(-data.BN_price);
            VE_buyCover.style.display = DisplayStyle.None;
            data.isBuyed = true;
            if (QuestManager.Instance.type == QuestType.UnlockMachine)
            {
                QuestManager.Instance.upQuest();
            }
            reloadUI();
        }
        else if (data.isBuyed) 
        {
            SoundManager.Instance.PlaySound(SoundEffectType.Forge);
            getProduction(true);


            if (this is machineIronElement)
            {
                if (QuestManager.Instance.type == QuestType.FarmWood)
                    QuestManager.Instance.upQuest(CalculReward());
            }
            if (this is machineUraniumElement)
            {
                if (QuestManager.Instance.type == QuestType.FarmUranium)
                    QuestManager.Instance.upQuest(CalculReward());
            }
            if (!Stats.Instance.dialogues["FirstMachineClick"])
            {
                Stats.Instance.ironUnlocked = true;
                Stats.Instance.dialogues["FirstMachineClick"] = true;
                DialogueManager.Instance.ExecuteBlock("FirstMachineClick");
            }

            Datas.Instance.current.machineClicked += 1;
        }
    }

    private void getProduction(bool launch)
    {
        HandleMoney(CalculReward());
        if (launch) LauncherMarker();
    }

    protected virtual void LevelUp()
    {
        if ((!canBuy(CalculLevelUpCost()) || !havelevel() ) || data.color == borderColor.black ) return;
        HandleMoney(-CalculLevelUpCost());
        data.level += data.multiplicator;

        data.multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, getLimitLevel() - data.level);


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

        LoadLockBorders();
        Datas.Instance.current.upgradeBuy += 1;
        //SetBorderColor();
    }

    
    public virtual void Update(Rect scrollRect)
    {
        if (!data.isBuyed) return;

        if (data.production_cps > 0) {
            data.time += Time.deltaTime;
            if (data.time >= (1.0f / (float)data.production_cps)){
                getProduction(IsVisibleInScrollView(scrollRect));
                data.time = 0f;
            }
        }
    }

    #endregion

    #region ------ calculs methods ------ 

    public int getMulitplicator()
    {
        return Mathf.Min(data.levelMax - data.level, UpMode.Instance.upModeMultiplicator);
    }

    protected BigNumber CalculLevelUpCost()
    {
        BigNumber calculedNumber = new BigNumber(0);

        int mult = getMulitplicator();
        double r = 1.25;
        double pow = System.Math.Pow(r, data.level);

        BigNumber priceModifier = data.BN_price * 0.01f;
        if (priceModifier < new BigNumber(1)) priceModifier.Set(1);

        calculedNumber.Set(priceModifier * Stats.Instance.upgradesPriceReducer * 15f); //price * 
        calculedNumber.Multiply(pow, false); //1.75 ** level


        double factor = (System.Math.Pow(r, mult) - 1) / (r - 1);//calcule de la suite géométrique
        calculedNumber.Multiply(factor, false);

        calculedNumber.Normalize();
        return calculedNumber;
    }

    public BigNumber CalculColorTempPrice(borderColor color)
    {
        int level = machineData.levelColor[(int)color + 1];
        BigNumber price = new BigNumber(data.BN_price * 0.01f);
        if (price < new BigNumber(1)) price.Set(1);

        double factor = 15.00 * System.Math.Pow(1.25, level) * Stats.Instance.upgradesPriceReducer;

        price.Multiply(factor, false);

        price.Normalize();
        return price;
    }

    public int CalculColorLifePrice(borderColor color)
    {
        if ((int)color == 1) return 25;
        if(data.borderbuys[color] == BorderBuyType.permanent) return 0;
        return 25 + CalculColorLifePrice(color - 1);
    }

    private int GetColorAmount()
    {
        int i = 0;
        foreach (int lvColor in machineData.levelColor)
        {
            if (lvColor > data.level  && lvColor <= data.level + getMulitplicator())
            {
                i++;
            }
        }
        return i;
    }
    public BigNumber CalculReward() { return CalculReward(data.level); }

    public BigNumber CalculReward(int lvl)
    {
        BigNumber reward = new BigNumber(1);
        reward.Multiply(Mathf.Pow(1.175f, lvl)); //  1.2^reallevel * ( 0.5 * initialTIme^2 )
        reward.Add(lvl - 1);
        if (Settings.Instance.showBanner) reward *= Consts.BANNER_REWARD;

        BigNumber machinePriceModifier = data.BN_price * 0.00085f;
        if (machinePriceModifier > new BigNumber(1)) reward *= machinePriceModifier;


        if (Stats.Instance.boosts.ContainsKey(Boost.Type.damage))
        {
            if (Stats.Instance.boosts[Boost.Type.ressources].time > 0f)
                reward *= Stats.Instance.boosts[Boost.Type.ressources].coef;
        }
        reward *= Ship.Current.ressourceMultiplier;
        reward.round();

        reward.Normalize();
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
        VE_lockedLevelCover.style.visibility = havelevel(data.level + (getMulitplicator() - 1))? Visibility.Hidden : Visibility.Visible;
        Lbl_upCost.text = CalculLevelUpCost().ToString();
    }

    private bool havelevel()
    {
        return havelevel(data.level);
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
        int targetLevel = data.level + mult;
        int requiredShipLevel = Mathf.CeilToInt(targetLevel / 2f) - 1;

        return Mathf.Max(0, requiredShipLevel);
    }

    #endregion

    #region ------ adaptativeStyle ------
    protected void SetBorderColor()
    {

        foreach (var val in data.borderbuys)
        {
            if (val.Value == BorderBuyType.unbuyed) continue;
            data.color = val.Key;
        }

        StyleSheet blackBorderStyle = Resources.Load<StyleSheet>("styles/machineBlackBorderStyle");
        StyleSheet styleSheet = Resources.Load<StyleSheet>("styles/machineStyle");

        if (data.color == borderColor.black)
        {
            styleSheets.Add(blackBorderStyle);
            styleSheets.Remove(styleSheet);


            string pathCadre = "machines/" + data.color.ToString() + "/cadre";
            string pathButton = "machines/" + data.color.ToString() + "/button";

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

        

        style.unityBackgroundImageTintColor = Consts.BORDERS_COLORS[(int)data.color];
        Btn_up.style.unityBackgroundImageTintColor = Consts.BORDERS_COLORS[(int)data.color];
        VE_logo.style.unityBackgroundImageTintColor = Consts.BORDERS_COLORS[(int)data.color];

        data.production_cps = Consts.BORDER_EMPLOYEE[(int)data.color];
    }

    public bool IsVisibleInScrollView(Rect scrollRect)
    {
        if (panel == null || scrollRect == null) return false;

        Rect rect = this.worldBound;
        Rect machineRect = new Rect(
            rect.x, 
            rect.y,
            rect.width,
            rect.height * 0.25f
        );
        return machineRect.Overlaps(scrollRect);
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

    protected virtual Color getColor()
    {
        return Color.white;
    }

    protected virtual void SetLogo()
    {

    }

    protected virtual void LauncherMarker()
    {

    }

    #endregion

    #region ------ comparateurs ------
    public override bool Equals(object obj)
    {
        if (obj is machineElement other)
            return data.machineName == other.data.machineName;

        return false;
    }

    public override int GetHashCode()
    {
        return data.machineName.GetHashCode();
    }
#endregion
}
