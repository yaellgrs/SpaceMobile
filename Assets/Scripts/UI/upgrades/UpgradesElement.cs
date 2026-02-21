using System.Drawing;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UpgradesElement : VisualElement
{
    //attributs
    #region ----- UI Elements -----

    //main
    protected Label Lbl_name;
    protected VisualElement VE_logo;
        protected Label Lbl_level;
    protected Label Lbl_description;

    //upButton
    protected Button Btn_levelUp;
    protected Label Lbl_levelUpText;
    protected Label Lbl_levelUpCost;
        protected VisualElement VE_levelUpCostLogo;
    protected VisualElement VE_levelUpLockCover;
    protected Label Lbl_levelUpLockLevel;

    #endregion

    #region ----- variables -----

    protected int level = 1;
    protected int levelMax = 100;

    private int multiplicator = 1;
    protected string upgradesname = "name";


    #endregion

    //methods
    #region ----- constructors -----
    public UpgradesElement()
    {
        Init();
    }

    public UpgradesElement(string name)
    {
        this.name = name;
        Init();
    }

    #endregion

    #region ----- INIT -----

    protected virtual void Init()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("styles/upgradeStyle");
        styleSheets.Add(styleSheet);

        AddToClassList("upgradeCadre");
        InitMain();
        InitLevelUp();
    }

    private void InitMain()
    {
        Lbl_name = new Label();
        VE_logo = new VisualElement();
        Lbl_level = new Label();
        Lbl_description = new Label();

        Lbl_name.text = name;
        Lbl_level.text = "lv : 50 (+1000)";
        Lbl_description.text = "Damage : 10";

        Lbl_name.AddToClassList("upgradeName");
        VE_logo.AddToClassList("upgradeLogo");
        Lbl_level.AddToClassList("upgradeLevel");
        Lbl_description.AddToClassList("upgradeDescription");


        Add(Lbl_name);
        Add(Lbl_description);
        Add(VE_logo);
        Add(Lbl_level);
    }

    private void InitLevelUp()
    {
        Btn_levelUp = new Button();
        Lbl_levelUpText = new Label();
        Lbl_levelUpCost = new Label();
        VE_levelUpCostLogo = new VisualElement();
        VE_levelUpLockCover = new VisualElement();
        Lbl_levelUpLockLevel = new Label();

        Lbl_levelUpText.text = "UPGRADE";
        Lbl_levelUpCost.text = "100";
        Lbl_levelUpLockLevel.text = "5";

        Btn_levelUp.AddToClassList("upgradeLevelUpButton");
        Btn_levelUp.AddToClassList("forgeButton");
        Lbl_levelUpText.AddToClassList("upgradeLevelUpText");
        Lbl_levelUpCost.AddToClassList("upgradeLevelUpCost");
        VE_levelUpCostLogo.AddToClassList("upgradeLevelUpCostLogo");
        VE_levelUpLockCover.AddToClassList("upgradeLevelUpLocked");
        Lbl_levelUpLockLevel.AddToClassList("upgradeLevelUpLockedLevel");
        //Lbl_levelUpCost.AddToClassList("uraniumColor");

        Add(Btn_levelUp);
        Btn_levelUp.Add(Lbl_levelUpText);
        Btn_levelUp.Add(Lbl_levelUpCost);
            Lbl_levelUpCost.Add(VE_levelUpCostLogo);
        Btn_levelUp.Add(VE_levelUpLockCover);
            VE_levelUpLockCover.Add(Lbl_levelUpLockLevel);
    }

    #endregion

    #region ----- Loads ----
    public void Load()
    {
        if (level < Stats.Instance.MinimalLevel && this is not UpgradesPrestigeElement) level = Stats.Instance.MinimalLevel;

        LoadStat();
        LoadUI();
        SetReward();
        SetLogos();

        Lbl_levelUpCost.text = CalculLevelUpCost().ToString();
        Btn_levelUp.clicked -= LevelUp;
        Btn_levelUp.clicked += LevelUp;

        SetLevelUpButton();
    }


    public void LoadUI()
    {
        multiplicator = Mathf.Min(UpMode.Instance.upModeMultiplicator, levelMax - level);

        //check if the player can upgrade
        bool havelevel = haveLevel();   
        VE_levelUpLockCover.style.visibility = havelevel ? Visibility.Hidden : Visibility.Visible;

        LoadLevelUI();
        Lbl_levelUpLockLevel.text = (Ship.Current.level + 2).ToString();
        Lbl_levelUpCost.text = CalculLevelUpCost().ToString();
    }

    public virtual void LoadLevelUI()
    {
        if (level >= levelMax) //LEVEL MAX
        {
            Lbl_level.text = "lv : MAX";
            Lbl_levelUpCost.style.display = DisplayStyle.None;
        }
        else
        {
            Lbl_level.text = $"lv {level.ToString()}/{levelMax.ToString()} <color=cyan>(+{getMulitplicator()})</color>";
            Lbl_levelUpCost.style.display = DisplayStyle.Flex;
        }
    }

    public virtual bool haveLevel()
    {
        return level < Ship.Current.level + 1;
    }

    protected virtual void SetLevelUpButton()
    {
        Btn_levelUp.enabledSelf = CanPay() && level < Ship.Current.level + 1;

    }
    #endregion

    #region ----- main workflow ----



    protected virtual void LevelUp()
    {
        if (level < levelMax)
        {
            PayCost();
            multiplicator = UpMode.Instance.upModeMultiplicator;

            if (level + multiplicator >= levelMax)
            {
                level = levelMax;
            }
            else level += multiplicator;

            SetReward();

            //set in load donc j'ai commenté
            //Lbl_level.text = (level == levelMax) ? "MAX" : level.ToString() + "/" + levelMax.ToString();
        }
        Load();
        gameManager.instance.SmallVibrate();
    }   

    #endregion

    #region ----- Calculs Methods ----

    protected BigNumber CalculLevelUpCost()
    {
        BigNumber calculedNumber = new BigNumber(1, 0);

        double r = 1.60;
        double pow = System.Math.Pow(r, level); // début de la suite
        multiplicator = Mathf.Max(1, multiplicator);
        calculedNumber.Set(50);
        calculedNumber.Multiply(pow, false);   
        calculedNumber.Multiply(Stats.Instance.upgradesPriceReducer, false);   
        double factor = (System.Math.Pow(r, multiplicator) - 1) / (r - 1);
        calculedNumber.Multiply(factor, false);

        calculedNumber.Normalize();
        return calculedNumber;
    }

    public int getMulitplicator()
    {
        return Mathf.Min(levelMax - level, UpMode.Instance.upModeMultiplicator);
    }

    #endregion

    #region ----- virtuals Methods ----

    protected virtual void LoadStat()
    {

    }

    protected virtual string getStat()
    {
        return "";
    }

    public virtual void SetReward()
    {

    }

    public virtual BigNumber GetReward(int lvl)
    {
        return new BigNumber(0);
    }


    protected virtual bool CanPay()
    {
        return false;
    }

    protected virtual void PayCost()
    {   

    }

    protected virtual void SetLogos()
    {

    }

    #endregion


    #region ------ comparateurs ------
    public override bool Equals(object obj)
    {
        if (obj is UpgradesElement other)
            return name == other.name;

        return false;
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }
    #endregion
}

