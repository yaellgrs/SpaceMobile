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
        levelMax = int.MaxValue;
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
            UpgradeType.DamageMultiplicator => Ship.Current.damage.prestige_multiplicator.ToString("F2"),
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

    public override void LoadLevelUI()
    {
        string levevelTxt = "lv " + level.ToString();
        if (Utility.HaveTheShipUpgrade(UpgradesShipElement.UpgradeType.AdditionalLevel))
        {
            levevelTxt += " (+" + Stats.Instance.shipUpgradesReward[UpgradesShipElement.UpgradeType.AdditionalLevel] + " )"; 
        }
        Lbl_level.text = levevelTxt;
        Lbl_levelUpCost.style.display = DisplayStyle.Flex;
    }

    public override bool haveLevel()
    {
        return true;
    }
    public override bool haveLevel(int lv)
    {
        return true;
    }

    public override void SetReward()
    {
        int realLevel = level;
        if (Utility.HaveTheShipUpgrade(UpgradesShipElement.UpgradeType.AdditionalLevel))
        {
            realLevel += (int)Stats.Instance.shipUpgradesReward[UpgradesShipElement.UpgradeType.AdditionalLevel];
        }

        switch (type)
        {
            case UpgradeType.PrestigeMultiplicator:
                Stats.Instance.star_multiplicator_prestige = 1f + 0.15f * (realLevel - 1);
                break;
            case UpgradeType.LessMeteor:
                Stats.Instance.enemyPerStage = 10f - 0.16f * (realLevel);
                break;
            case UpgradeType.LessPriceUpgrades:
                Stats.Instance.upgradesPriceReducer = 1f - 0.229f * Mathf.Log(realLevel);
                break;
            case UpgradeType.XpBoost:
                Stats.Instance.XpMultiplicator = 1f + 0.25f * (realLevel);
                break;
            case UpgradeType.DamageMultiplicator:
                Ship.Current.damage.prestige_multiplicator = 1f + 0.2f * (realLevel);
                break;
            case UpgradeType.StageSkip:
                Stats.Instance.stageSkipProb = realLevel;
                break;
            case UpgradeType.OmegaProb:
                Stats.Instance.probabilitéOfOmega = (realLevel + 1) * 5;
                break;
            case UpgradeType.MinimumLevel:
                Stats.Instance.MinimalLevel = realLevel;
                break;
            case UpgradeType.CriticalProbability:
                Stats.Instance.critical_Prob = realLevel * 5f;
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
        //if(!Stats.Instance.starPariticul.isBigger(CalculLevelUpCost())) Debug.LogError("You can't paye : " + name);
        return Stats.Instance.starPariticul.isBigger(CalculLevelUpCost());
    }

    protected override void SetLevelUpButton()
    {
        Btn_levelUp.enabledSelf = CanPay();

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
