using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using static Machine;



[UxmlElement]
public partial class UpgradesElement : VisualElement
{
    //attributs
    #region ----- UI Elements -----

    //main
    private Label Lbl_name;
    private VisualElement VE_logo;
        private Label Lbl_level;
    private Label Lbl_description;

    //upButton
    private Button Btn_levelUp;
    private Label Lbl_levelUpText;
    private Label Lbl_levelUpCost;
        private VisualElement VE_levelUpCostLogo;
    private VisualElement VE_levelUpLockCover;
    private Label Lbl_levelUpLockLevel;

    #endregion

    #region ----- variables -----

    #endregion

    //methods
    #region ----- constructors -----
    public UpgradesElement()
    {
        Init();
    }

    #endregion

    #region ----- INIT -----

    private void Init()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("styles/upgradeStyle");
        styleSheets.Remove(styleSheet);

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

        Lbl_name.text = "name";
        Lbl_level.text = "1/10";
        Lbl_description.text = "damage : 15";

        Lbl_name.AddToClassList("upgradeName");
        VE_logo.AddToClassList("upgradeLogo");
        Lbl_level.AddToClassList("upgradeLevel");
        Lbl_description.AddToClassList("upgradeDescription");

        Add(Lbl_name);
        Add(Lbl_description);
        Add(VE_logo);
        VE_logo.Add(Lbl_level);
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
        Lbl_levelUpText.AddToClassList("upgradeLevelUpText");
        Lbl_levelUpCost.AddToClassList("upgradeLevelUpCost");
        VE_levelUpCostLogo.AddToClassList("upgradeLevelUpCostLogo");
        VE_levelUpLockCover.AddToClassList("upgradeLevelUpLocked");
        Lbl_levelUpLockLevel.AddToClassList("upgradeLevelUpLockedLevel");

        Add(Btn_levelUp);
        Btn_levelUp.Add(Lbl_levelUpText);
        Btn_levelUp.Add(Lbl_levelUpCost);
            Lbl_levelUpCost.Add(VE_levelUpCostLogo);
        Btn_levelUp.Add(VE_levelUpLockCover);
            VE_levelUpLockCover.Add(Lbl_levelUpLockLevel);
    }

    #endregion

    #region ----- main workflow ----


    #endregion

    #region ----- virtuals Methods ----

    #endregion
}
