using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum UpgradeType { PrestigeMultiplicator, LessMeteor, LessPriceUpgrades, XpBoost, DamageMultiplicator, StageSkip, OmegaProb, MinimumLevel, CriticalProbability, Max };

public class UpgradesPrestigeElement : UpgradesElement
{
    #region ----- variables -----
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
        string key = "Prestige_upgrade_" + type;

        //string logo_path = "prestige/";

        string logoPath = "Upgrades/prestige/";
        Texture2D logoTexutre = Resources.Load<Texture2D>(logoPath + type);
        if (logoTexutre == null) logoTexutre = Resources.Load<Texture2D>(logoPath + "CadresBlanc");
        VE_logo.style.backgroundImage = logoTexutre;

        Lbl_name.text = type.ToString();


        str = type switch
        {
            UpgradeType.PrestigeMultiplicator => Stats.Instance.star_multiplicator_prestige.ToString("F2"),
            UpgradeType.LessMeteor => Stats.Instance.enemyPerStage.ToString("F2"),
            UpgradeType.LessPriceUpgrades => Stats.Instance.upgradesPriceReducer.ToString("F2"),
            UpgradeType.XpBoost => Stats.Instance.XpMultiplicator.ToString("F2"),
            UpgradeType.DamageMultiplicator => Stats.Instance.prest_damage_multiplicator.ToString("F2"),
            UpgradeType.StageSkip => Stats.Instance.stageSkipProb.ToString("F2"),
            UpgradeType.OmegaProb => Stats.Instance.probabilitéOfOmega.ToString("F2"),
            UpgradeType.MinimumLevel => Stats.Instance.MinimalLevel.ToString("F2"),
            UpgradeType.CriticalProbability => Stats.Instance.critical_Prob.ToString("F2"),
            _ => "",
        };

        if (LocalizationSettings.SelectedLocale == null)
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];

        LocalizedString localizeUpgrades = new LocalizedString("UI_Upgrades", key);

        localizeUpgrades.Arguments = new object[] { str };
        localizeUpgrades.StringChanged += (localizeValue) =>
        {
            Lbl_description.text = localizeValue.ToString();
        };
        localizeUpgrades.RefreshString();
    }

    public override void GetReward()
    {
        switch (type)
        {
            case UpgradeType.PrestigeMultiplicator:
                Stats.Instance.star_multiplicator_prestige = 1f + 0.15f * (level - 1);
                break;
            case UpgradeType.LessMeteor:
                Stats.Instance.enemyPerStage = 10f - 0.16f * (level);
                break;
            case UpgradeType.LessPriceUpgrades:
                Stats.Instance.upgradesPriceReducer = 1f - 0.229f * Mathf.Log(level);
                break;
            case UpgradeType.XpBoost:
                Stats.Instance.XpMultiplicator = 1f + 0.25f * (level);
                break;
            case UpgradeType.DamageMultiplicator:
                Ship.Current.damage.prestige_multiplicator = 1f + 0.2f * (level);
                break;
            case UpgradeType.StageSkip:
                Stats.Instance.prest_damage_multiplicator = level;
                break;
            case UpgradeType.OmegaProb:
                Stats.Instance.probabilitéOfOmega = (level + 1) * 5;
                break;
            case UpgradeType.MinimumLevel:
                Stats.Instance.MinimalLevel = level;
                break;
            case UpgradeType.CriticalProbability:
                Stats.Instance.critical_Prob = level * 5f;
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
