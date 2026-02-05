using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class shipUpgradeElement : Button
{
    Label Lbl_name;
    Label Lbl_price;
    VisualElement VE_logoPrice;
    VisualElement VE_logo;
    VisualElement VE_progressCadre;
    VisualElement VE_progressBar;
    VisualElement VE_progressBarHidder;

    Button Btn_buy;

    public int _level;

    [UxmlAttribute]
    public SpaceShipType type;

    [UxmlAttribute]
    public int Level
    {
        get => _level;
        set
        {
            _level = Mathf.Clamp(value, 0, 5);
            SetShipLevel(_level);
        }
    }

    public shipUpgradeElement()
    {
        AddToClassList("ShipElement");
        AddToClassList("button");

        Lbl_name = new Label();
        VE_logoPrice = new VisualElement();
        VE_logo = new VisualElement();
        VE_progressCadre = new VisualElement();
        VE_progressBar = new VisualElement();
        VE_progressBarHidder = new VisualElement();
        Lbl_price = new Label();
        Btn_buy = new Button();

        Lbl_name.AddToClassList("ShipName");
        VE_logoPrice.AddToClassList("ShipLogoPrice");
        VE_logo.AddToClassList("ShipLogo");
        VE_progressCadre.AddToClassList("ShipProgressCadre");
        VE_progressBar.AddToClassList("ShipProgressBar");
        Lbl_price.AddToClassList("ShipPrice");
        Btn_buy.AddToClassList("ShipBuy");
        Btn_buy.AddToClassList("button");

        Lbl_name.text = "Basic SpaceShip";
        Btn_buy.text = "UP";
        Lbl_price.text = "0/10";

        Add( Lbl_name );
        Add(VE_logo);
        Add(Lbl_price);
        Add(Btn_buy);
        Add(VE_progressBar);
        VE_progressBar.Add(VE_progressCadre);
        Lbl_price.Add(VE_logoPrice);

        clicked += SwitchShip;


    }

    public void SetShipLevel(int level)
    {
        string path = "ship/progresBarShipLevel" + level;
        Texture2D tex = Resources.Load<Texture2D>(path);
        VE_progressBar.style.backgroundImage = new StyleBackground(tex);

    }

    private void SwitchShip()
    {
        if(ShipManager.Instance != null)
        {
            ShipManager.Instance.SwitchShip(type);
        }
    }
 

}
