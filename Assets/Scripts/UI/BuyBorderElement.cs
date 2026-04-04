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

    #endregion


    public BuyBorderElement()
    {
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

        InitText();
        InitBuyButton();

        Add(Lbl_description);
        Add(Btn_buy);
    }

    private void InitText()
    {
        //temp
        BorderBuyType type = BorderBuyType.unbuyed;
        borderColor color = borderColor.bronze;
        //temp

        int employee = Consts.BORDER_EMPLOYEE[(int)color];

        Lbl_description.text  = type switch {
            BorderBuyType.unbuyed => "Nombre d'employée embauchée pour le voyage : " + employee,
            BorderBuyType.temp => "Nombre d'employée embauchée à vie : " + employee,
            _ => "Les employées sont déjà au travail.",
        };
    }

    private void InitBuyButton()
    {
        //temp
        BorderBuyType type = BorderBuyType.unbuyed;
        BigNumber cost = new BigNumber(5, 3);
        //temp

        if(type == BorderBuyType.permanent) Btn_buy.style.display = DisplayStyle.None;

        string logo_path = type == BorderBuyType.temp ? "logos/diamand" :
                                                        "logos/mainRessource/" + Ship.Current.type.ToString() + "/mainRessource";


        Btn_buy.text = cost.ToString();
        Sprite icon = Resources.Load<Sprite>(logo_path);
        if(icon != null) 
            Btn_buy.iconImage = Background.FromSprite(icon);
        else Debug.LogError("Icon not found at path: " + logo_path);
    }
}
