using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class UpgradesShipElement : UpgradesElement
{
    #region ----- variables -----
    public enum UpgradeType { AdditionalLevel, DpsBooster, truc };

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

    #region ----- overrides Methods -----

    protected override void LoadStat()
    {
        string str = "";
        string key = "Prestige_upgrade_" + type;

        //string logo_path = "prestige/";

        string logoPath = "Upgrades/prestige/";
        Texture2D logoTexutre = Resources.Load<Texture2D>(logoPath + type);
        if (logoTexutre == null) logoTexutre = Resources.Load<Texture2D>(logoPath + "CadresBlanc");
        VE_logo.style.backgroundImage = logoTexutre;

        Lbl_name.text = type.ToString();

        Lbl_description.text = "no defined for instance";
/*        str = type switch
        {
            UpgradeType.PrestigeMultiplicator => Stats.Instance.star_multiplicator_prestige.ToString("F2"),
            UpgradeType.LessMeteor => Stats.Instance.enemyPerStage.ToString("F2"),
            UpgradeType.LessTimeMachine => Stats.Instance.machineTimeReducer.ToString("F2"),
            UpgradeType.LessPriceUpgrades => Stats.Instance.upgradesPriceReducer.ToString("F2"),
            UpgradeType.XpBoost => Stats.Instance.XpMultiplicator.ToString("F2"),
            UpgradeType.DamageMultiplicator => Stats.Instance.prest_damage_multiplicator.ToString("F2"),
            UpgradeType.StageSkip => Stats.Instance.stageSkipProb.ToString("F2"),
            UpgradeType.OmegaProb => Stats.Instance.probabilitéOfOmega.ToString("F2"),
            UpgradeType.MinimumLevel => Stats.Instance.MinimalLevel.ToString("F2"),
            _ => "",
        };*/

/*        if (LocalizationSettings.SelectedLocale == null)
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];

        LocalizedString localizeUpgrades = new LocalizedString("UI_Upgrades", key);

        localizeUpgrades.Arguments = new object[] { str };
        localizeUpgrades.StringChanged += (localizeValue) =>
        {
            Lbl_description.text = localizeValue.ToString();
        };
        localizeUpgrades.RefreshString();*/
    }

    public override void GetReward()
    {
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
