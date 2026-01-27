using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class UpgradesPrestigeElement : UpgradesElement
{
    #region ----- variables -----
    public enum UpgradeType { PrestigeMultiplicator, LessMeteor, LessTimeMachine, LessPriceUpgrades, XpBoost, DamageMultiplicator, StageSkip, OmegaProb, Max };
    public UpgradeType type;
    #endregion

    #region ----- Constructors -----
    public UpgradesPrestigeElement() : base()
    {

    }

    public UpgradesPrestigeElement(string name, UpgradeType type) : base()
    {
        this.name = name;
        this.type = type;
    }
    #endregion

    #region ----- overrides Methods -----

    protected override void LoadStat()
    {
        string str = "";
        string key = "Prestige_upgrade_";

        //string logo_path = "prestige/";

        string logoPath = "Upgrades/prestige/";
        Texture2D logoTexutre = Resources.Load<Texture2D>(logoPath + type);
        if (logoTexutre == null) logoTexutre = Resources.Load<Texture2D>(logoPath + "CadresBlanc");
        VE_logo.style.backgroundImage = logoTexutre;

        switch (type)
        {
            case UpgradeType.PrestigeMultiplicator://pas de logo
                key += "PrestigeMultiplicator";
                str = Stats.Instance.star_multiplicator_prestige.ToString("F2");
                Lbl_name.text = "PrestigeMultiplicator";
                break;
            case UpgradeType.LessMeteor://pas de logo
                key += "LessMeteor";
                str = Stats.Instance.enemyPerStage.ToString("F2");
                Lbl_name.text = "LessMeteor";
                break;
            case UpgradeType.LessTimeMachine://pas de logo
                key += "LessTimeMachine";
                str = Stats.Instance.machineTimeReducer.ToString("F2");
                Lbl_name.text = "LessTimeMachine";
                break;
            case UpgradeType.LessPriceUpgrades://pas de logo
                key += "LessPriceUpgrades";
                str = Stats.Instance.upgradesPriceReducer.ToString("F2");
                Lbl_name.text = "LessPriceUpgrades";
                break;
            case UpgradeType.XpBoost://pas de logo
                key += "XpBoost";
                str = Stats.Instance.XpMultiplicator.ToString("F2");
                Lbl_name.text = "XpBoost";
                break;
            case UpgradeType.DamageMultiplicator:
                key += "DamageMultiplicator";
                str = Stats.Instance.prest_damage_multiplicator.ToString("F2");
                Lbl_name.text = "DamageMultiplicator";
                break;
            case UpgradeType.StageSkip://pas de logo
                key += "StageSkip";
                str = Stats.Instance.stageSkipProb.ToString("F0") + "%";
                Lbl_name.text = "StageSkip";
                break;
            case UpgradeType.OmegaProb:
                key += "OmegaProb";
                str = Stats.Instance.probabilitéOfOmega.ToString("F0") + "%";
                Lbl_name.text = "OmegaProb";
                break;
            case UpgradeType.Max:
                Lbl_name.text = "Max";
                break;
        }

        if (LocalizationSettings.SelectedLocale == null)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }

        LocalizedString localizeUpgrades = new LocalizedString("UI_Upgrades", key);

        localizeUpgrades.Arguments = new object[] { str };
        localizeUpgrades.StringChanged += (localizeValue) =>
        {
            Lbl_description.text = localizeValue.ToString();
        };
        localizeUpgrades.RefreshString();

/*        string logo_path = "Upgrades/Iron/" + type.ToString();
        VE_logo.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(logo_path));*/
    }

    protected override void GetReward()
    {
        switch (type)
        {
            case UpgradeType.PrestigeMultiplicator:
                Stats.Instance.star_multiplicator_prestige = 1f + 0.15f * (level - 1);
                break;
            case UpgradeType.LessMeteor:
                Stats.Instance.enemyPerStage = 10f - 0.16f * (level);
                break;
            case UpgradeType.LessTimeMachine:
                Stats.Instance.machineTimeReducer = 1f - 0.229f * Mathf.Log(level);
                break;
            case UpgradeType.LessPriceUpgrades:
                Stats.Instance.upgradesPriceReducer = 1f - 0.229f * Mathf.Log(level);
                break;
            case UpgradeType.XpBoost:
                Stats.Instance.XpMultiplicator = 1f + 0.25f * (level);
                break;
            case UpgradeType.DamageMultiplicator:
                Stats.Instance.prest_damage_multiplicator = 1f + 0.2f * (level);
                break;
            case UpgradeType.StageSkip:
                Stats.Instance.prest_damage_multiplicator = level;
                break;
            case UpgradeType.OmegaProb:
                Stats.Instance.probabilitéOfOmega = (level + 1) * 5;
                break;
        }
       LoadStat();

    }
    protected override void PayCost()
    {
        Stats.Instance.addPrestige(-CalculLevelUpCost());
    }

    protected override bool CanPay()
    {
        return Stats.Instance.starPariticul.isBigger(CalculLevelUpCost());
    }

    protected override void SetLogos()
    {
        Texture2D logoTexture = Resources.Load<Texture2D>("logos/prestige");
        StyleBackground background = new StyleBackground(logoTexture);
        VE_levelUpCostLogo.style.backgroundImage = background;
        Lbl_levelUpCost.AddToClassList("prestigeColor");
    }
    #endregion
}
