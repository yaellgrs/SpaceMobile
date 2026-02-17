using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class UpgradesShipElement : UpgradesElement
{
    #region ----- variables -----
    public enum UpgradeType { AdditionalLevel, Magnectic, DamageOverTime, ZoneDamage };

    public UpgradeType type;
    #endregion

    #region ----- Constructors -----
    public UpgradesShipElement() : base()
    {

    }

    public UpgradesShipElement(string name, UpgradeType type) : base()
    {
        this.name = name;
        this.type = type;
    }
    #endregion

    public bool isUnlocked()
    {
        return (int)type < (int)Ship.Current.type;
    }

    #region ----- overrides Methods -----

    protected override void LoadStat()
    {
        //string str = "";
        //string key = "Prestige_upgrade_" + type;

        //string logo_path = "prestige/";

        string logoPath = "Upgrades/prestige/";
        Texture2D logoTexutre = Resources.Load<Texture2D>(logoPath + type);
        if (logoTexutre == null) logoTexutre = Resources.Load<Texture2D>(logoPath + "CadresBlanc");
        VE_logo.style.backgroundImage = logoTexutre;

        Lbl_name.text = type.ToString();

        Lbl_description.text = "no defined";
    }

    public override void GetReward()
    {
        if (!isUnlocked()) return;

        switch (type)
        {

        }
       LoadStat();

    }
    protected override void PayCost()
    {
        Stats.Instance.AddShipMoney(-CalculLevelUpCost(), false);
    }

    protected override bool CanPay()
    {
        return Stats.Instance.BN_shipMoney.isBigger(CalculLevelUpCost());
    }

    protected override void SetLogos()
    {
/*        Texture2D logoTexture = Resources.Load<Texture2D>("logos/prestige");
        StyleBackground background = new StyleBackground(logoTexture);
        VE_levelUpCostLogo.style.backgroundImage = background;
        Lbl_levelUpCost.AddToClassList("prestigeColor");*/
    }
    #endregion
}
