using UnityEngine;
using UnityEngine.UIElements;

public class UpgradesUraniumElement : UpgradesElement
{
    #region ----- variables -----
    public enum UpgradeType { SpeedAuto, AreaSlow, AreaWidth, RocketReload, RocketMultiplier }
    public UpgradeType type;
    #endregion

    #region ----- Constructors -----
    public UpgradesUraniumElement() : base()
    {

    }

    public UpgradesUraniumElement(string name, UpgradeType type) : base()
    {
        this.name = name;
        this.type = type;
    }
    #endregion

    #region ----- overrides Methods -----

    protected override void LoadStat()
    {
        switch (type)
        {
            case UpgradeType.SpeedAuto:
                Lbl_description.text = "Shoot/s : " + ((1f / (Stats.Instance.speedAuto))).ToString("F2");
                break;
            case UpgradeType.AreaSlow:
                Lbl_description.text = "meteors speed : x" + (1f / Stats.Instance.areaSpeed).ToString("F2");
                break;
            case UpgradeType.AreaWidth:
                Lbl_description.text = "Area Width : " + Stats.Instance.areaSize.ToString("F2");
                break;
            case UpgradeType.RocketReload:
                Lbl_description.text = "Time to reload : " + Stats.Instance.rocketTimerMax.ToString("F2");
                break;
            case UpgradeType.RocketMultiplier:
                Lbl_description.text = "Damage : x" + Stats.Instance.rocketMultiplier.ToString("F2");
                break;
        }
        //Lbl_description

        string logo_path = "Upgrades/Iron/" + type.ToString();
        VE_logo.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(logo_path));
    }

    public override void GetReward()
    {
        switch (type)
        {
            case UpgradeType.SpeedAuto:
                Stats.Instance.speedAuto = 1f / (0.09f * (level + 1));
                Debug.Log("speed auto shoot : " + Stats.Instance.speedAuto);
                break;
            case UpgradeType.AreaSlow:
                Stats.Instance.areaSpeed = 1f + 0.5f * Mathf.Pow(level + 1, 0.6f);
                break;
            case UpgradeType.AreaWidth:
                Stats.Instance.areaSize = 1f + 0.3f * Mathf.Pow(level, 0.4f);
                spaceShip.instance.setAreaScale();
                break;
            case UpgradeType.RocketReload:
                Stats.Instance.rocketTimerMax = 25f - Mathf.Pow(level, 0.4f);
                break;
            case UpgradeType.RocketMultiplier:
                Ship.Current.damage.rocket_multiplicator = 5f + 0.25f * Mathf.Pow(level - 1, 1.15f);
                break;
        }

        LoadStat();
    }
    protected override void PayCost()
    {
        Stats.Instance.AddUranium(-CalculLevelUpCost());
    }

    protected override bool CanPay()
    {
        return Ship.Current.uranium.isBigger(CalculLevelUpCost());
    }

    protected override void SetLogos()
    {
        Texture2D logoTexture = Resources.Load<Texture2D>("logos/uranium");
        StyleBackground background = new StyleBackground(logoTexture);
        VE_levelUpCostLogo.style.backgroundImage = background;
        Lbl_levelUpCost.AddToClassList("uraniumColor");
        Debug.Log("set logos for uranium");
    }
    #endregion
}
