using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UIElements;

public enum BorderBuyType
{
    unbuyed, temp, permanent
}   

[UxmlElement]
public partial class BuyBorderElement : VisualElement
{
    #region --- Elements ---

    private Label Lbl_description;
    private Button Btn_buy;

    private machineElement machine;
    private borderColor borderColor = borderColor.bronze;
    private BorderBuyType buyType = BorderBuyType.temp;

    #endregion


    public BuyBorderElement()
    {
        Init();
    }

    public BuyBorderElement(machineElement machine, borderColor borderColor, BorderBuyType buyType)
    {
        this.machine = machine;
        this.borderColor = borderColor;
        this.buyType = buyType;
        Init();
    }

    private void Init()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("styles/BuyBorderStyle");
        styleSheets.Add(styleSheet);

        Lbl_description = new Label();
        Btn_buy = new Button();


        AddToClassList("BuyBorder");
        Lbl_description.AddToClassList("BuyBorderDescription");
        Btn_buy.AddToClassList("BuyBorderBuy");
        Btn_buy.AddToClassList("button");



        Add(Lbl_description);
        Add(Btn_buy);

        Load();
    }

    private void Load()
    {
        InitText();
        InitBuyButton();
        InitColor();

        if (canBuy())
        {
            Btn_buy.clicked -= Buy;
            Btn_buy.clicked += Buy;
            Btn_buy.SetEnabled(true);
        }
        else
        {
            Btn_buy.SetEnabled(false);
        }
    }

    private void InitColor()
    {
        Utility.setBorderColor(this, Consts.BORDERS_COLORS[(int)borderColor]);
        Utility.setBorderColor(Btn_buy, Consts.BORDERS_COLORS[(int)borderColor]);
    }

    private void InitText()
    {
        //temp
        //temp

        int employee = Consts.BORDER_EMPLOYEE[(int)borderColor];

        Lbl_description.text  = buyType switch {
            BorderBuyType.unbuyed => "Nombre d'employée a embauchée pour le voyage : " + employee,
            BorderBuyType.temp => "Nombre d'employée a embauchée à vie : " + employee,
            _ => "Les " + employee + " employées sont déjà au travail.",
        };
    }

    private void InitBuyButton()
    {
        //temp
        BigNumber cost;
        if (machine == null) cost = new BigNumber(1, 3);
        else if(buyType == BorderBuyType.unbuyed)cost = machine.CalculColorTempPrice(borderColor);
        else cost = new BigNumber(machine.CalculColorLifePrice(borderColor));
        //temp

        if(buyType == BorderBuyType.permanent) Btn_buy.style.display = DisplayStyle.None;

        string logo_path = buyType == BorderBuyType.temp ? "logos/diamand" :
                                                        "logos/mainRessource/" + Ship.Current.type.ToString() + "/mainRessource";


        Btn_buy.text = cost.ToString();
        Sprite icon = Resources.Load<Sprite>(logo_path);
        if(icon != null) 
            Btn_buy.iconImage = Background.FromSprite(icon);
        else Debug.LogError("Icon not found at path: " + logo_path);
    }

    private bool canBuy()
    {
        if (machine == null) return false;
        if(buyType == BorderBuyType.unbuyed) return Ship.Current.iron >= machine.CalculColorTempPrice(borderColor);
        if(buyType == BorderBuyType.temp) return Stats.Instance.diamand >= machine.CalculColorLifePrice(borderColor);
        return false;
    }

    private void Buy()
    {
        if (machine == null) return;
        if (buyType == BorderBuyType.unbuyed) Stats.Instance.AddIron(-machine.CalculColorTempPrice(borderColor));
        else if (buyType == BorderBuyType.temp)Stats.Instance.AddDiamand(-machine.CalculColorLifePrice(borderColor));

        BorderBuyType newType = buyType + 1;

        foreach (var val in machine.data.borderbuys.ToList())
        {
            if (val.Key <= borderColor && val.Value < newType) machine.data.borderbuys[val.Key] = newType;
        }

        BorderUI.Instance?.Load();
        machine.LoadMachine();
        Load();

    }
}
