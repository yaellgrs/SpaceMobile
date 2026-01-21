using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class shipUpgradeElement : VisualElement
{
    Label Lbl_name;
    Label Lbl_price;
    VisualElement VE_logoPrice;
    VisualElement VE_logo;

    Button Btn_buy;

    public shipUpgradeElement()
    {
        AddToClassList("ShipElement");

        Lbl_name = new Label();
        VE_logoPrice = new VisualElement();
        VE_logo = new VisualElement();
        Lbl_price = new Label();
        Btn_buy = new Button();

        Lbl_name.AddToClassList("ShipName");
        VE_logoPrice.AddToClassList("ShipLogoPrice");
        VE_logo.AddToClassList("ShipLogo");
        Lbl_price.AddToClassList("ShipPrice");
        Btn_buy.AddToClassList("ShipBuy");

        Lbl_name.text = "Basic SpaceShip";
        Btn_buy.text = "buy";
        Lbl_price.text = "0/10";

        Add( Lbl_name );
        Add(VE_logo);
        Lbl_price.Add(VE_logoPrice);
        Add(Lbl_price);
        Add(Btn_buy);
    }

}
